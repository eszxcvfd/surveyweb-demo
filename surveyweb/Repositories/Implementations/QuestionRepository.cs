using Microsoft.EntityFrameworkCore;
using SurveyWeb.Data.Models;
using SurveyWeb.Repositories.Interfaces;

namespace SurveyWeb.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly SurveyDbContext _ctx;

        public QuestionRepository(SurveyDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<question>> GetBySurveyAsync(Guid surveyId)
        {
            return await _ctx.questions
                .Where(q => q.surveyId == surveyId)
                .OrderBy(q => q.orderNo)
                .ToListAsync();
        }

        public async Task<question?> GetByIdAsync(Guid id)
        {
            return await _ctx.questions
                .FirstOrDefaultAsync(q => q._id == id);
        }

        public async Task AddAsync(question question)
        {
            await _ctx.questions.AddAsync(question);
        }

        public Task UpdateAsync(question question)
        {
            _ctx.questions.Update(question);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _ctx.questions.FirstOrDefaultAsync(q => q._id == id);
            if (entity != null)
            {
                _ctx.questions.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<questionType>> GetAllQuestionTypesAsync()
        {
            return await _ctx.questionTypes
                .OrderBy(qt => qt.categoryId)
                .ThenBy(qt => qt.displayName)
                .ToListAsync();
        }

        public async Task AddOptionsAsync(List<questionOption> options)
        {
            if (options != null && options.Any())
            {
                await _ctx.questionOptions.AddRangeAsync(options);
            }
        }

        public async Task<IReadOnlyList<questionOption>> GetQuestionOptionsByQuestionIdAsync(Guid questionId)
        {
            return await _ctx.questionOptions
                .Where(o => o.questionId == questionId)
                .OrderBy(o => o.orderNo)
                .ToListAsync();
        }

        public async Task DeleteQuestionOptionsByQuestionIdAsync(Guid questionId)
        {
            var options = await _ctx.questionOptions
                .Where(o => o.questionId == questionId)
                .ToListAsync();

            if (options.Any())
            {
                _ctx.questionOptions.RemoveRange(options);
            }
        }

        // Logic support
        public async Task<logicRule?> GetRuleByTargetAsync(Guid targetQuestionId)
        {
            return await _ctx.logicRules
                .Include(r => r.logicRuleConditions)
                .FirstOrDefaultAsync(r => r.targetId == targetQuestionId);
        }

        public async Task<logicRule?> GetRuleBySourceAsync(Guid sourceQuestionId, string logicType)
        {
            return await _ctx.logicRules
                .Include(r => r.logicRuleConditions)
                .FirstOrDefaultAsync(r => r.sourceId == sourceQuestionId && r.logicType == logicType);
        }

        public async Task<List<logicRule>> GetDisplayRulesBySourceAsync(Guid parentQuestionId)
        {
            return await _ctx.logicRules
                .Include(r => r.logicRuleConditions)
                .Where(r => r.sourceId == parentQuestionId && r.logicType == "display")
                .ToListAsync();
        }

        public async Task UpsertRuleAsync(logicRule rule)
        {
            var existing = await _ctx.logicRules
                .FirstOrDefaultAsync(r => r._id == rule._id);

            if (existing == null)
            {
                if (rule._id == Guid.Empty) rule._id = Guid.NewGuid();
                await _ctx.logicRules.AddAsync(rule);
            }
            else
            {
                existing.surveyId = rule.surveyId;
                existing.sourceId = rule.sourceId;
                existing.targetId = rule.targetId;
                existing.logicType = rule.logicType;
                existing.targetType = rule.targetType;
                existing.conditionGroup = rule.conditionGroup;
                existing.action = rule.action;
                existing.isActive = rule.isActive;
                existing.priority = rule.priority;
                existing.description = rule.description;
                _ctx.logicRules.Update(existing);
            }
        }

        public async Task ReplaceConditionsAsync(Guid ruleId, IEnumerable<logicRuleCondition> conditions)
        {
            var current = await _ctx.logicRuleConditions
                .Where(c => c.ruleId == ruleId)
                .ToListAsync();

            if (current.Any())
            {
                _ctx.logicRuleConditions.RemoveRange(current);
            }

            if (conditions != null && conditions.Any())
            {
                foreach (var c in conditions)
                {
                    // EF will use ruleId provided
                    await _ctx.logicRuleConditions.AddAsync(c);
                }
            }
        }

        public async Task DeleteRulesForQuestionAsync(Guid questionId)
        {
            var rules = await _ctx.logicRules
                .Where(r => r.sourceId == questionId || r.targetId == questionId)
                .ToListAsync();

            if (rules.Any())
            {
                var ruleIds = rules.Select(r => r._id).ToList();
                var conds = await _ctx.logicRuleConditions
                    .Where(c => ruleIds.Contains(c.ruleId))
                    .ToListAsync();

                if (conds.Any())
                    _ctx.logicRuleConditions.RemoveRange(conds);

                _ctx.logicRules.RemoveRange(rules);
            }
        }

        public async Task<IReadOnlyList<logicRuleCondition>> GetConditionsByRuleIdAsync(Guid ruleId)
        {
            return await _ctx.logicRuleConditions
                .Where(c => c.ruleId == ruleId)
                .OrderBy(c => c.conditionId)
                .ToListAsync();
        }
    }
}
