namespace SuggestionsServiceDemo.Domain.Models;

/// <summary>
/// Instance of a suggested partner company.
/// </summary>
/// <param name="CompanyId">The ID of the suggested partner company.</param>
public record CompanySuggestion(int CompanyId)
{
    /// <summary>
    /// The state of the suggestion made.
    /// </summary>
    public CompanySuggestionState State { get; set; } = CompanySuggestionState.Pending;
}
