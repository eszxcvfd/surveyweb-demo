using SurveyWeb.Data.Models;

namespace SurveyWeb.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<question>> GetBySurveyAsync(Guid surveyId);
        Task<question?> GetByIdAsync(Guid id);
        Task AddAsync(question question);
        Task UpdateAsync(question question);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
        Task<IEnumerable<questionType>> GetAllQuestionTypesAsync();
        Task AddOptionsAsync(List<questionOption> options);
    }
}