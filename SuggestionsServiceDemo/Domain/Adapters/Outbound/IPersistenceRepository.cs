using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Domain.Adapters.Outbound;

/// <summary>
/// Abstraction of outbound calls to the Persistence Repository (DB).
/// </summary>
public interface IPersistenceRepository
{
    /// <summary>
    /// Retrieves all suggestions for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <returns>If company and suggestions exist it will return a collection of all, else <c>null</c>.</returns>
    Task<IReadOnlyList<CompanySuggestion>?> GetCompanySuggestions(int companyId);

    /// <summary>
    /// Stores a collection of company suggestions for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="suggestions">Collection of related company suggestions.</param>
    Task StoreCompanySuggestions(int companyId, IReadOnlyList<CompanySuggestion> suggestions);

    /// <summary>
    /// Updates a specific company suggestion for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="updatedSuggestion">The updated company suggestion.</param>
    Task UpdateCompanySuggestion(int companyId, CompanySuggestion updatedSuggestion);

    /// <summary>
    /// Stores a collection of sequenced scheduled mail details for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="mailSequence">The sequence of scheduled mail details.</param>
    Task StoreMailSequence(int companyId, IReadOnlyList<ScheduledMailDetails> mailSequence);

    /// <summary>
    /// Retrieves the collection of sequenced scheduled mail details for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <returns>The sequence of scheduled mail details if it exists, else <c>null</c>.</returns>
    Task<IReadOnlyList<ScheduledMailDetails>?> GetMailSequence(int companyId);
}
