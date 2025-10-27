using SurveyWeb.Data.Models;

namespace SurveyWeb.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<question>> GetBySurveyAsync(Guid surveyId);
        Task<question?> GetByIdAsync(Guid id);
        Task AddAsync(question question);
        Task UpdateAsync(question question);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();

        Task<IEnumerable<questionType>> GetAllQuestionTypesAsync();

        Task AddOptionsAsync(List<questionOption> options);
        Task<IReadOnlyList<questionOption>> GetQuestionOptionsByQuestionIdAsync(Guid questionId);
        Task DeleteQuestionOptionsByQuestionIdAsync(Guid questionId);

        // Logic support
        Task<logicRule?> GetRuleByTargetAsync(Guid targetQuestionId); // e.g., display logic targets this question
        Task<logicRule?> GetRuleBySourceAsync(Guid sourceQuestionId, string logicType); // e.g., skip logic from this question
        Task<List<logicRule>> GetDisplayRulesBySourceAsync(Guid parentQuestionId); // children branches
        Task UpsertRuleAsync(logicRule rule);
        Task ReplaceConditionsAsync(Guid ruleId, IEnumerable<logicRuleCondition> conditions);
        Task DeleteRulesForQuestionAsync(Guid questionId); // delete any rule where sourceId==q or targetId==q
        Task<IReadOnlyList<logicRuleCondition>> GetConditionsByRuleIdAsync(Guid ruleId);
    }
}