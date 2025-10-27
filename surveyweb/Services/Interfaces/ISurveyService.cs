using SurveyWeb.Data.Models;

namespace SurveyWeb.Services.Interfaces
{
    public interface ISurveyService
    {
        Task<IEnumerable<survey>> GetAllAsync();
        Task<survey?> GetAsync(Guid id);
        Task<survey> CreateAsync(survey input);
        Task<bool> UpdateAsync(survey input);
        Task<bool> DeleteAsync(Guid id);
    }
}
