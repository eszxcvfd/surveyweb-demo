using SurveyWeb.Data.Models;

namespace SurveyWeb.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<question?> GetByIdAsync(Guid id);
        Task<IEnumerable<question>> GetBySurveyAsync(Guid surveyId);
        Task<IEnumerable<questionType>> GetAllQuestionTypesAsync();
        Task AddAsync(question entity);
        Task UpdateAsync(question entity);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task AddOptionsAsync(List<questionOption> options);
        
        // Thêm 2 methods mới
        Task<IReadOnlyList<questionOption>> GetQuestionOptionsByQuestionIdAsync(Guid questionId);
        Task DeleteQuestionOptionsByQuestionIdAsync(Guid questionId);
    }
}