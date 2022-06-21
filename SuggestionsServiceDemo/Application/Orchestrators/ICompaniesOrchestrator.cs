using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Application.Orchestrators;

/// <summary>
/// Orchestrates company-related workflows.
/// </summary>
public interface ICompaniesOrchestrator
{
    /// <summary>
    /// Create and persist suggested company partnerships for a given company.
    /// </summary>
    /// <param name="company">The given company.</param>
    Task GenerateCompanySuggestions(Company company);

    /// <summary>
    /// Gets all suggested company partnerships for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="filter">An optional filter to apply to the collection of data being returned.</param>
    /// <returns>The collection of suggested company partnerships that fulfill the filter predicate (if it exists).</returns>
    Task<IReadOnlyList<CompanySuggestion>> GetAllCompanySuggestions(int companyId, Predicate<CompanySuggestion>? filter = null);

    /// <summary>
    /// Gets details on a specific suggested company partnership for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="suggestedCompanyId">The ID of the suggested company.</param>
    /// <returns>The details of the company suggestion.</returns>
    Task<CompanySuggestion> GetCompanySuggestion(int companyId, int suggestedCompanyId);

    /// <summary>
    /// Updates the details of a specific suggested company partnership for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="updatedCompanySuggestion">The updated company suggestion.</param>
    Task UpdateCompanySuggestion(int companyId, CompanySuggestion updatedCompanySuggestion);
}
