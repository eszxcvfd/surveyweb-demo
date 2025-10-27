using SurveyWeb.Data.Models;

namespace SurveyWeb.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<IReadOnlyList<question>> GetSurveyQuestionsAsync(Guid surveyId);
        Task<question?> GetQuestionByIdAsync(Guid questionId);
        Task<IReadOnlyList<questionType>> GetQuestionTypesAsync();
        Task<question> CreateQuestionAsync(Guid surveyId, question entity);
        Task<bool> DeleteQuestionAsync(Guid questionId);
        Task AddQuestionOptionsAsync(Guid questionId, IEnumerable<string> options);
        Task<IReadOnlyList<string>> GetQuestionOptionsAsync(Guid questionId);

        Task<bool> UpdateQuestionAsync(
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
            // new fields
            string? allowedMime,
            int? maxFiles,
            int? maxFileSizeMB,
            string? matrixRowsJson,
            string? matrixColsJson,
            string? scaleLabelsJson,
            bool aiProbeEnabled
        );

        Task ReplaceQuestionOptionsAsync(Guid questionId, IEnumerable<string> options);
    }
}
