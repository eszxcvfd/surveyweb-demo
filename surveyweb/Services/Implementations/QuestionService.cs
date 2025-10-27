using Microsoft.EntityFrameworkCore;
using SurveyWeb.Data.Models;
using SurveyWeb.Repositories.Interfaces;
using SurveyWeb.Services.Interfaces;

namespace SurveyWeb.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;

        public QuestionService(IQuestionRepository questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<IReadOnlyList<question>> GetSurveyQuestionsAsync(Guid surveyId)
        {
            var questions = await _questionRepo.GetBySurveyAsync(surveyId);
            return questions.ToList();
        }

        public async Task<question?> GetQuestionByIdAsync(Guid questionId)
        {
            return await _questionRepo.GetByIdAsync(questionId);
        }

        public async Task<IReadOnlyList<questionType>> GetQuestionTypesAsync()
        {
            var types = await _questionRepo.GetAllQuestionTypesAsync();
            return types.ToList();
        }

        public async Task<question> CreateQuestionAsync(Guid surveyId, question entity)
        {
            if (entity._id == Guid.Empty)
                entity._id = Guid.NewGuid();

            entity.surveyId = surveyId;

            await _questionRepo.AddAsync(entity);
            await _questionRepo.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> DeleteQuestionAsync(Guid questionId)
        {
            // Delete dependent options and logic first
            await _questionRepo.DeleteQuestionOptionsByQuestionIdAsync(questionId);
            await _questionRepo.DeleteRulesForQuestionAsync(questionId);

            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                return false;

            await _questionRepo.DeleteAsync(questionId);
            await _questionRepo.SaveChangesAsync();
            return true;
        }

        public async Task AddQuestionOptionsAsync(Guid questionId, IEnumerable<string> options)
        {
            var optionsList = options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
            var questionOptions = new List<questionOption>();

            for (int i = 0; i < optionsList.Count; i++)
            {
                var opt = new questionOption
                {
                    _id = Guid.NewGuid(),
                    questionId = questionId,
                    optionText = optionsList[i],
                    orderNo = i + 1
                };
                questionOptions.Add(opt);
            }

            await _questionRepo.AddOptionsAsync(questionOptions);
            await _questionRepo.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<string>> GetQuestionOptionsAsync(Guid questionId)
        {
            var options = await _questionRepo.GetQuestionOptionsByQuestionIdAsync(questionId);
            return options.OrderBy(o => o.orderNo)
                         .Select(o => o.optionText ?? "")
                         .ToList();
        }

        public async Task<bool> UpdateQuestionAsync(
            Guid id,
            string? text,
            int orderNo,
            bool isRequired,
            int? questionTypeId,
            string? typeCode,
            double? minValue,
            double? maxValue,
            double? step,
            int? maxChars,
            string? allowedMime,
            int? maxFiles,
            int? maxFileSizeMB,
            string? matrixRowsJson,
            string? matrixColsJson,
            string? scaleLabelsJson,
            bool aiProbeEnabled)
        {
            var existing = await _questionRepo.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.text = text ?? existing.text;
            existing.orderNo = orderNo;
            existing.isRequired = isRequired;
            existing.questionTypeId = questionTypeId;
            existing.type = string.IsNullOrWhiteSpace(typeCode) ? existing.type : typeCode;

            existing.minValue = minValue;
            existing.maxValue = maxValue;
            existing.step = step;
            existing.maxChars = maxChars;

            existing.allowedMime = allowedMime;
            existing.maxFiles = maxFiles;
            existing.maxFileSizeMB = maxFileSizeMB;

            existing.matrixRowsJson = matrixRowsJson;
            existing.matrixColsJson = matrixColsJson;
            existing.scaleLabelsJson = scaleLabelsJson;

            existing.aiProbeEnabled = aiProbeEnabled;

            await _questionRepo.UpdateAsync(existing);
            await _questionRepo.SaveChangesAsync();

            return true;
        }

        public async Task ReplaceQuestionOptionsAsync(Guid questionId, IEnumerable<string> options)
        {
            await _questionRepo.DeleteQuestionOptionsByQuestionIdAsync(questionId);
            await AddQuestionOptionsAsync(questionId, options);
        }

        // Logic
        public async Task<logicRule?> GetLogicForQuestionAsync(Guid questionId)
        {
            // Prefer target-bound rule (display/display-option). If not found, try source-bound skip.
            var targetRule = await _questionRepo.GetRuleByTargetAsync(questionId);
            if (targetRule != null) return targetRule;
            var sourceSkip = await _questionRepo.GetRuleBySourceAsync(questionId, "skip");
            return sourceSkip;
        }

        public async Task<(logicRule? Rule, logicRuleCondition? Condition)> GetLogicForQuestionWithConditionAsync(Guid questionId)
        {
            // Ưu tiên rule target-bound (display/display_option), nếu không có thì thử skip theo source
            var rule = await _questionRepo.GetRuleByTargetAsync(questionId)
                       ?? await _questionRepo.GetRuleBySourceAsync(questionId, "skip");

            if (rule == null) return (null, null);

            var conditions = await _questionRepo.GetConditionsByRuleIdAsync(rule._id);
            var cond = conditions.OrderBy(c => c.conditionId).FirstOrDefault();
            return (rule, cond);
        }

        public async Task UpsertDisplayLogicAsync(Guid surveyId, Guid targetQuestionId, Guid sourceQuestionId,
            string op, string? rightText, double? rightNumber, DateTime? rightDate, Guid? rightOptionId)
        {
            await _questionRepo.DeleteRulesForQuestionAsync(targetQuestionId);

            var rule = new logicRule
            {
                _id = Guid.NewGuid(),
                surveyId = surveyId,
                sourceId = sourceQuestionId,
                targetId = targetQuestionId,
                logicType = "display",
                targetType = "question",
                isActive = true,
                priority = 1
            };

            await _questionRepo.UpsertRuleAsync(rule);
            await _questionRepo.SaveChangesAsync();

            var cond = new logicRuleCondition
            {
                ruleId = rule._id,
                questionId = sourceQuestionId,
                optionId = rightOptionId,
                leftOperandType = "answer",
                _operator = op,
                rightValueText = rightText,
                rightValueNumber = rightNumber,
                rightValueDate = rightDate,
                groupConnector = "AND"
            };

            await _questionRepo.ReplaceConditionsAsync(rule._id, new[] { cond });

            // Xóa subtype (x.1, x.2, ...) nếu đang có
            var child = await _questionRepo.GetByIdAsync(targetQuestionId);
            if (child != null)
            {
                child.subtype = null;
                await _questionRepo.UpdateAsync(child);
            }

            await _questionRepo.SaveChangesAsync();
        }

        public async Task UpsertSkipLogicAsync(Guid surveyId, Guid sourceQuestionId, Guid targetQuestionId,
            string op, string? rightText, double? rightNumber, DateTime? rightDate, Guid? rightOptionId)
        {
            // delete rules bound to this question (as source or target) then add skip
            await _questionRepo.DeleteRulesForQuestionAsync(sourceQuestionId);

            var rule = new logicRule
            {
                _id = Guid.NewGuid(),
                surveyId = surveyId,
                sourceId = sourceQuestionId,
                targetId = targetQuestionId,
                logicType = "skip",
                targetType = "question",
                isActive = true,
                priority = 1
            };

            await _questionRepo.UpsertRuleAsync(rule);

            var cond = new logicRuleCondition
            {
                ruleId = rule._id,
                questionId = sourceQuestionId,
                optionId = rightOptionId,
                leftOperandType = "answer",
                _operator = op,
                rightValueText = rightText,
                rightValueNumber = rightNumber,
                rightValueDate = rightDate,
                groupConnector = "AND"
            };

            await _questionRepo.ReplaceConditionsAsync(rule._id, new[] { cond });

            await _questionRepo.SaveChangesAsync();
        }

        public async Task UpsertDisplayOptionLogicAsync(Guid surveyId, Guid questionId, Guid sourceQuestionId,
            IEnumerable<Guid> optionIdsToShow, string op, string? rightText, double? rightNumber, DateTime? rightDate, Guid? rightOptionId)
        {
            await _questionRepo.DeleteRulesForQuestionAsync(questionId);

            var rule = new logicRule
            {
                _id = Guid.NewGuid(),
                surveyId = surveyId,
                sourceId = sourceQuestionId,
                targetId = questionId,
                logicType = "display_option",
                targetType = "option",
                isActive = true,
                priority = 1,
                // store desired visible optionIds as CSV inside action for simplicity
                action = string.Join(",", optionIdsToShow ?? Enumerable.Empty<Guid>())
            };

            await _questionRepo.UpsertRuleAsync(rule);
            await _questionRepo.SaveChangesAsync(); // đảm bảo rule đã có trong DB

            var cond = new logicRuleCondition
            {
                ruleId = rule._id,
                questionId = sourceQuestionId,
                optionId = rightOptionId,
                leftOperandType = "answer",
                _operator = op,
                rightValueText = rightText,
                rightValueNumber = rightNumber,
                rightValueDate = rightDate,
                groupConnector = "AND"
            };

            await _questionRepo.ReplaceConditionsAsync(rule._id, new[] { cond });
            await _questionRepo.SaveChangesAsync();
        }

        public async Task DeleteLogicForQuestionAsync(Guid questionId)
        {
            await _questionRepo.DeleteRulesForQuestionAsync(questionId);
            await _questionRepo.SaveChangesAsync();
        }

        // Branch index manager: ensures subtype ("1","2",...) unique for children of parent
        public async Task EnsureBranchIndexAsync(Guid surveyId, Guid childQuestionId, Guid parentQuestionId)
        {
            var child = await _questionRepo.GetByIdAsync(childQuestionId);
            if (child == null) return;

            // All display rules that point from parent to some children
            var siblingsRules = await _questionRepo.GetDisplayRulesBySourceAsync(parentQuestionId);
            var siblingIds = siblingsRules.Select(r => r.targetId).Where(id => id.HasValue).Select(id => id!.Value).ToList();
            var siblings = new List<question>();

            foreach (var sid in siblingIds)
            {
                var s = await _questionRepo.GetByIdAsync(sid);
                if (s != null) siblings.Add(s);
            }

            // Parse existing subtype numbers
            var used = new HashSet<int>();
            foreach (var s in siblings)
            {
                if (s._id == childQuestionId) continue;
                if (int.TryParse(s.subtype, out var num) && num > 0)
                    used.Add(num);
            }

            // If child has valid subtype and not used, keep it; else assign next
            if (!int.TryParse(child.subtype, out var childIndex) || childIndex <= 0 || used.Contains(childIndex))
            {
                var next = 1;
                while (used.Contains(next)) next++;
                child.subtype = next.ToString();
                await _questionRepo.UpdateAsync(child);
                await _questionRepo.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<QuestionOptionPair>> GetQuestionOptionsWithIdsAsync(Guid questionId)
        {
            var options = await _questionRepo.GetQuestionOptionsByQuestionIdAsync(questionId);
            return options
                .OrderBy(o => o.orderNo)
                .Select(o => new QuestionOptionPair(o._id, o.optionText ?? string.Empty))
                .ToList();
        }
    }
}