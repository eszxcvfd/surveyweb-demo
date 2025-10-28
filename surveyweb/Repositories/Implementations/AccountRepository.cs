using Microsoft.EntityFrameworkCore;
using SurveyWeb.Data.Models;
using SurveyWeb.Repositories.Interfaces;

namespace SurveyWeb.Repositories.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly SurveyDbContext _context;

    public AccountRepository(SurveyDbContext context)
    {
        _context = context;
    }

    public async Task<user?> GetUserByEmailAsync(string email)
    {
        return await _context.users
            .FirstOrDefaultAsync(u => u.email == email);
    }

    public async Task<user> CreateUserAsync(user user)
    {
        _context.users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.users
            .AnyAsync(u => u.email == email);
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var user = await _context.users.FindAsync(userId);
        if (user != null)
        {
            user.lastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
