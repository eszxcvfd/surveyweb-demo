using Microsoft.EntityFrameworkCore;
using SurveyWeb.Data.Models;
using SurveyWeb.Repositories.Interfaces;

namespace SurveyWeb.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly SurveyDbContext _ctx;

        public QuestionRepository(SurveyDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<question>> GetBySurveyAsync(Guid surveyId)
        {
            return await _ctx.questions
                .Where(q => q.surveyId == surveyId)
                .OrderBy(q => q.orderNo)
                .ToListAsync();
        }

        public async Task<question?> GetByIdAsync(Guid id)
        {
            return await _ctx.questions
                .FirstOrDefaultAsync(q => q._id == id);
        }

        public async Task AddAsync(question question)
        {
            await _ctx.questions.AddAsync(question);
        }

        public Task UpdateAsync(question question)
        {
            _ctx.questions.Update(question);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _ctx.questions.FirstOrDefaultAsync(q => q._id == id);
            if (entity != null)
            {
                _ctx.questions.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<questionType>> GetAllQuestionTypesAsync()
        {
            return await _ctx.questionTypes
                .OrderBy(qt => qt.categoryId)
                .ThenBy(qt => qt.displayName)
                .ToListAsync();
        }

        public async Task AddOptionsAsync(List<questionOption> options)
        {
            if (options != null && options.Any())
            {
                await _ctx.questionOptions.AddRangeAsync(options);
            }
        }
    }
}
