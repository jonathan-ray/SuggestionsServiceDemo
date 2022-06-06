using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Application.Orchestrators;

/// <summary>
/// Orchestrates mail-related workflows.
/// </summary>
public interface IMailOrchestrator
{
    /// <summary>
    /// Generates and persists the scheduled mail sequence details for a given company.
    /// </summary>
    /// <param name="company">The given company.</param>
    /// <returns>The scheduled mail sequence details.</returns>
    Task<IReadOnlyList<ScheduledMailDetails>> GenerateMailSequence(Company company);

    /// <summary>
    /// Gets the scheduled mail sequence details for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <returns>The scheduled mail sequence details.</returns>
    Task<IReadOnlyList<ScheduledMailDetails>> GetMailSequence(int companyId);

    /// <summary>
    /// Sends a mail to a company on the remaining pending company suggestions.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="mailTypeId">The type of mail to send.</param>
    /// <param name="pendingSuggestedCompanies">The remaining pending suggested companies.</param>
    Task SendPendingSuggestionsMail(int companyId, int mailTypeId, IReadOnlyList<CompanySuggestion> pendingSuggestedCompanies);
}
