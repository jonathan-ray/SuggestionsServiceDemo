using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Domain.Adapters.Outbound;

/// <summary>
/// Abstraction of outbound calls to the Growth Policy Service.
/// </summary>
public interface IGrowthPolicyService
{
    /// <summary>
    /// Gets the sequence of scheduled mails to be sent for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <returns>Sequence of scheduled mails.</returns>
    Task<IReadOnlyList<ScheduledMailDetails>> GetScheduledMailSequence(int companyId);

    /// <summary>
    /// Creates a mail for pending company suggestions for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="mailTypeId">The type of the mail to be sent.</param>
    /// <param name="pendingSuggestedCompanyIds">Collection of IDs of the suggested companies that are still pending.</param>
    /// <returns>Created details of a mail to be sent.</returns>
    Task<GroupMailItem> CreatePendingSuggestionsMail(int companyId, int mailTypeId, IReadOnlyList<int> pendingSuggestedCompanyIds);
}
