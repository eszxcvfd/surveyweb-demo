using SurveyWeb.Data.Models;
using SurveyWeb.Repositories.Interfaces;
using SurveyWeb.Services.Interfaces;

namespace SurveyWeb.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepo;
        private readonly ISurveyRepository _surveyRepo;

        public QuestionService(IQuestionRepository questionRepo, ISurveyRepository surveyRepo)
        {
            _questionRepo = questionRepo;
            _surveyRepo = surveyRepo;
        }

        public async Task<IEnumerable<question>> GetSurveyQuestionsAsync(Guid surveyId)
        {
            return await _questionRepo.GetBySurveyAsync(surveyId);
        }

        public async Task<IEnumerable<questionType>> GetQuestionTypesAsync()
        {
            return await _questionRepo.GetAllQuestionTypesAsync();
        }

        public async Task<question?> GetQuestionByIdAsync(Guid id)
        {
            return await _questionRepo.GetByIdAsync(id);
        }

        public async Task<question> CreateQuestionAsync(Guid surveyId, question input)
        {
            // Validate survey exists
            var survey = await _surveyRepo.GetByIdAsync(surveyId);
            if (survey == null)
            {
                throw new ArgumentException($"Survey with ID {surveyId} not found.");
            }

            // Set default values
            if (input._id == Guid.Empty)
                input._id = Guid.NewGuid();

            input.surveyId = surveyId;

            // If orderNo not set, get max order and add 1
            if (input.orderNo == 0)
            {
                var existingQuestions = await _questionRepo.GetBySurveyAsync(surveyId);
                input.orderNo = existingQuestions.Any() 
                    ? existingQuestions.Max(q => q.orderNo) + 1 
                    : 1;
            }

            await _questionRepo.AddAsync(input);
            await _questionRepo.SaveChangesAsync();

            return input;
        }

        public async Task<bool> UpdateQuestionAsync(Guid id, string? text, int orderNo, bool isRequired, int? questionTypeId)
        {
            var existing = await _questionRepo.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.text = text ?? string.Empty;
            existing.orderNo = orderNo;
            existing.isRequired = isRequired;
            existing.questionTypeId = questionTypeId;

            await _questionRepo.UpdateAsync(existing);
            await _questionRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteQuestionAsync(Guid id)
        {
            var existing = await _questionRepo.GetByIdAsync(id);
            if (existing == null)
                return false;

            await _questionRepo.DeleteAsync(id);
            await _questionRepo.SaveChangesAsync();

            return true;
        }

        public async Task AddQuestionOptionsAsync(Guid questionId, List<string> options)
        {
            if (options == null || !options.Any())
                return;

            var questionOptions = new List<questionOption>();
            
            for (int i = 0; i < options.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(options[i]))
                {
                    questionOptions.Add(new questionOption
                    {
                        _id = Guid.NewGuid(),
                        questionId = questionId,
                        optionText = options[i],
                        orderNo = i + 1,
                        isExclusive = false,
                        isOther = false
                    });
                }
            }

            if (questionOptions.Any())
            {
                await _questionRepo.AddOptionsAsync(questionOptions);
                await _questionRepo.SaveChangesAsync();
            }
        }
    }
}