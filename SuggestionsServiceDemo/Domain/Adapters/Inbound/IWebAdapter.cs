using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Domain.Adapters.Inbound;

/// <summary>
/// Abstraction of inbound calls from the Web Application.
/// </summary>
public interface IWebAdapter
{
    /// <summary>
    /// Updates the state of a company suggestion for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="suggestedCompanyId">The ID of the suggested company.</param>
    /// <param name="updatedState">The updated suggestion state.</param>
    Task UpdateCompanySuggestionState(int companyId, int suggestedCompanyId, CompanySuggestionState updatedState);
}
