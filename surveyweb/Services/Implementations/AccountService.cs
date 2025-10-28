using System.Security.Cryptography;
using System.Text;
using SurveyWeb.Data.Models;
using SurveyWeb.Models;
using SurveyWeb.Repositories.Interfaces;
using SurveyWeb.Services.Interfaces;

namespace SurveyWeb.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<(bool Success, string Message, user? User)> RegisterAsync(RegisterViewModel model)
    {
        // Kiểm tra email đã tồn tại
        if (await _accountRepository.EmailExistsAsync(model.Email))
        {
            return (false, "Email đã được sử dụng", null);
        }

        // Tạo user mới
        var newUser = new user
        {
            _id = Guid.NewGuid(),
            fullName = model.FullName,
            email = model.Email,
            passwordHash = HashPassword(model.Password),
            role = "user",
            status = "active",
            createdAt = DateTime.UtcNow,
            lastLogin = null
        };

        try
        {
            var createdUser = await _accountRepository.CreateUserAsync(newUser);
            return (true, "Đăng ký thành công", createdUser);
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi khi tạo tài khoản: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string Message, user? User)> LoginAsync(LoginViewModel model)
    {
        var user = await _accountRepository.GetUserByEmailAsync(model.Email);

        if (user == null)
        {
            return (false, "Email hoặc mật khẩu không đúng", null);
        }

        if (user.status != "active")
        {
            return (false, "Tài khoản đã bị vô hiệu hóa", null);
        }

        if (!VerifyPassword(model.Password, user.passwordHash))
        {
            return (false, "Email hoặc mật khẩu không đúng", null);
        }

        // Cập nhật thời gian đăng nhập
        await _accountRepository.UpdateLastLoginAsync(user._id);

        return (true, "Đăng nhập thành công", user);
    }

    public byte[] HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public bool VerifyPassword(string password, byte[] passwordHash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword.SequenceEqual(passwordHash);
    }
}
