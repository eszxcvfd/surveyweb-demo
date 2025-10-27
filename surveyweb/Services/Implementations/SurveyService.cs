using SurveyWeb.Data.Models;
using SurveyWeb.Repositories.Interfaces;
using SurveyWeb.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SurveyWeb.Services.Implementations
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _repo;

        public SurveyService(ISurveyRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<survey>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<survey?> GetAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<survey> CreateAsync(survey input)
        {
            // Đảm bảo _id là GUID mới nếu chưa có
            if (input._id == Guid.Empty)
                input._id = Guid.NewGuid();

            // Đảm bảo CreatedAt có giá trị
            if (input.createdAt == DateTime.MinValue)
                input.createdAt = DateTime.UtcNow;

            // Nếu Status rỗng => để "draft"
            if (string.IsNullOrWhiteSpace(input.status))
                input.status = "draft";

            // Clear navigation properties để tránh EF tracking issues
            input.owner = null!;
            input.distributionChannels = new List<distributionChannel>();
            input.exportJobs = new List<exportJob>();
            input.invitations = new List<invitation>();
            input.logicRules = new List<logicRule>();
            input.questions = new List<question>();
            input.reportViews = new List<reportView>();
            input.responses = new List<response>();
            input.sections = new List<section>();
            input.securityEvents = new List<securityEvent>();
            input.surveyPermissions = new List<surveyPermission>();

            await _repo.AddAsync(input);
            await _repo.SaveChangesAsync();

            return input;
        }

        public async Task<bool> UpdateAsync(survey input)
        {
            var existing = await _repo.GetByIdAsync(input._id);
            if (existing == null)
                return false;

            // copy các field cho phép update
            existing.title = input.title;
            existing.description = input.description;
            existing.status = input.status;
            existing.isAnonymous = input.isAnonymous;
            existing.preventDuplicate = input.preventDuplicate;
            existing.maxResponses = input.maxResponses;
            existing.openAt = input.openAt;
            existing.closeAt = input.closeAt;

            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}


