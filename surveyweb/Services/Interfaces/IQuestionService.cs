using SurveyWeb.Data.Models;

namespace SurveyWeb.Services.Interfaces
{
    public record QuestionOptionPair(Guid Id, string Text);

    public interface IQuestionService
    {
        Task<IReadOnlyList<question>> GetSurveyQuestionsAsync(Guid surveyId);
        Task<question?> GetQuestionByIdAsync(Guid questionId);
        Task<IReadOnlyList<questionType>> GetQuestionTypesAsync();
        Task<question> CreateQuestionAsync(Guid surveyId, question entity);
        Task<bool> DeleteQuestionAsync(Guid questionId);

        Task AddQuestionOptionsAsync(Guid questionId, IEnumerable<string> options);
        Task<IReadOnlyList<string>> GetQuestionOptionsAsync(Guid questionId);
        Task<IReadOnlyList<QuestionOptionPair>> GetQuestionOptionsWithIdsAsync(Guid questionId);

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
            string? allowedMime,
            int? maxFiles,
            int? maxFileSizeMB,
            string? matrixRowsJson,
            string? matrixColsJson,
            string? scaleLabelsJson,
            bool aiProbeEnabled);

        Task ReplaceQuestionOptionsAsync(Guid questionId, IEnumerable<string> options);

        // Logic
        Task<logicRule?> GetLogicForQuestionAsync(Guid questionId);
        Task<(logicRule? Rule, logicRuleCondition? Condition)> GetLogicForQuestionWithConditionAsync(Guid questionId);
        Task UpsertDisplayLogicAsync(Guid surveyId, Guid targetQuestionId, Guid sourceQuestionId,
            string op, string? rightText, double? rightNumber, DateTime? rightDate, Guid? rightOptionId);
        Task UpsertSkipLogicAsync(Guid surveyId, Guid sourceQuestionId, Guid targetQuestionId,
            string op, string? rightText, double? rightNumber, DateTime? rightDate, Guid? rightOptionId);
        Task UpsertDisplayOptionLogicAsync(Guid surveyId, Guid questionId, Guid sourceQuestionId,
            IEnumerable<Guid> optionIdsToShow, string op, string? rightText, double? rightNumber, DateTime? rightDate, Guid? rightOptionId);
        Task DeleteLogicForQuestionAsync(Guid questionId);

        // Branching index management
        Task EnsureBranchIndexAsync(Guid surveyId, Guid childQuestionId, Guid parentQuestionId);
    }
}
