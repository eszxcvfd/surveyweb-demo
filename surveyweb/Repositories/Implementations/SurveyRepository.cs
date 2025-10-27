using Microsoft.EntityFrameworkCore;
using SurveyWeb.Data.Models;
using SurveyWeb.Repositories.Interfaces;
using System.Diagnostics;

namespace SurveyWeb.Repositories.Implementations
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly SurveyDbContext _ctx;

        public SurveyRepository(SurveyDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<survey>> GetAllAsync()
        {
            // lấy tối đa 100 bản ghi mới nhất
            return await _ctx.surveys
                .OrderByDescending(s => s.createdAt)
                .Take(100)
                .ToListAsync();
        }

        public async Task<survey?> GetByIdAsync(Guid id)
        {
            Debug.WriteLine($"Repository: Searching for survey with ID: {id}");
            Console.WriteLine($"Repository: Searching for survey with ID: {id}");

            var result = await _ctx.surveys
                .FirstOrDefaultAsync(s => s._id == id);

            if (result == null)
            {
                Debug.WriteLine($"Repository: Survey NOT FOUND");
                Console.WriteLine($"Repository: Survey NOT FOUND");
                
                // Debug: List tất cả surveys trong DB
                var allSurveys = await _ctx.surveys.Select(s => new { s._id, s.title }).ToListAsync();
                Debug.WriteLine($"Repository: Total surveys in DB: {allSurveys.Count}");
                Console.WriteLine($"Repository: Total surveys in DB: {allSurveys.Count}");
                foreach (var s in allSurveys)
                {
                    Debug.WriteLine($"  - {s._id} | {s.title}");
                    Console.WriteLine($"  - {s._id} | {s.title}");
                }
            }
            else
            {
                Debug.WriteLine($"Repository: Survey FOUND - {result.title}");
                Console.WriteLine($"Repository: Survey FOUND - {result.title}");
            }

            return result;
        }

        public async Task AddAsync(survey survey)
        {
            await _ctx.surveys.AddAsync(survey);
        }

        public Task UpdateAsync(survey survey)
        {
            _ctx.surveys.Update(survey);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _ctx.surveys.FirstOrDefaultAsync(s => s._id == id);
            if (entity != null)
            {
                _ctx.surveys.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }
    }
}
