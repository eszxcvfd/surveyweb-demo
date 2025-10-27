using SurveyWeb.Data.Models;

namespace SurveyWeb.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<IEnumerable<question>> GetSurveyQuestionsAsync(Guid surveyId);
        Task<IEnumerable<questionType>> GetQuestionTypesAsync();
        Task<question> CreateQuestionAsync(Guid surveyId, question input);
        Task<question?> GetQuestionByIdAsync(Guid id);
        Task<bool> UpdateQuestionAsync(Guid id, string? text, int orderNo, bool isRequired, int? questionTypeId);
        Task<bool> DeleteQuestionAsync(Guid id);
        Task AddQuestionOptionsAsync(Guid questionId, List<string> options);
    }
}
