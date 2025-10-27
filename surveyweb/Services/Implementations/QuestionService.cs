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
            => (await _questionRepo.GetBySurveyAsync(surveyId)).ToList();

        public Task<question?> GetQuestionByIdAsync(Guid questionId)
            => _questionRepo.GetByIdAsync(questionId);

        public async Task<IReadOnlyList<questionType>> GetQuestionTypesAsync()
            => (await _questionRepo.GetAllQuestionTypesAsync()).ToList();

        public async Task<question> CreateQuestionAsync(Guid surveyId, question entity)
        {
            if (entity._id == Guid.Empty) entity._id = Guid.NewGuid();
            entity.surveyId = surveyId;
            await _questionRepo.AddAsync(entity);
            await _questionRepo.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteQuestionAsync(Guid questionId)
        {
            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                return false;

            // Xóa tất cả questionOptions trước khi xóa question
            await _questionRepo.DeleteQuestionOptionsByQuestionIdAsync(questionId);

            // Sau đó xóa question
            await _questionRepo.DeleteAsync(questionId);
            await _questionRepo.SaveChangesAsync();
            
            return true;
        }

        public async Task AddQuestionOptionsAsync(Guid questionId, IEnumerable<string> options)
        {
            var list = options.Where(o => !string.IsNullOrWhiteSpace(o)).ToList();
            var entities = new List<questionOption>();
            for (int i = 0; i < list.Count; i++)
            {
                entities.Add(new questionOption
                {
                    _id = Guid.NewGuid(),
                    questionId = questionId,
                    optionText = list[i],
                    orderNo = i + 1
                });
            }
            await _questionRepo.AddOptionsAsync(entities);
            await _questionRepo.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<string>> GetQuestionOptionsAsync(Guid questionId)
        {
            var opts = await _questionRepo.GetQuestionOptionsByQuestionIdAsync(questionId);
            return opts.OrderBy(o => o.orderNo).Select(o => o.optionText ?? "").ToList();
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
            if (existing == null) return false;

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
    }
}