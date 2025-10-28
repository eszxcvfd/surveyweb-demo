using SurveyWeb.Data.Models;

namespace SurveyWeb.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<user?> GetUserByEmailAsync(string email);
    Task<user> CreateUserAsync(user user);
    Task<bool> EmailExistsAsync(string email);
    Task UpdateLastLoginAsync(Guid userId);
}
