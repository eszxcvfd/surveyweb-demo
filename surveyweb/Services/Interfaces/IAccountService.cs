using SurveyWeb.Data.Models;
using SurveyWeb.Models;

namespace SurveyWeb.Services.Interfaces;

public interface IAccountService
{
    Task<(bool Success, string Message, user? User)> RegisterAsync(RegisterViewModel model);
    Task<(bool Success, string Message, user? User)> LoginAsync(LoginViewModel model);
    byte[] HashPassword(string password);
    bool VerifyPassword(string password, byte[] passwordHash);
}
