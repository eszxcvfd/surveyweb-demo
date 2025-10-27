using SurveyWeb.Data.Models;

namespace SurveyWeb.Repositories.Interfaces
{
    public interface ISurveyRepository
    {
        Task<IEnumerable<survey>> GetAllAsync();
        Task<survey?> GetByIdAsync(Guid id);
        Task AddAsync(survey survey);
        Task UpdateAsync(survey survey);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
