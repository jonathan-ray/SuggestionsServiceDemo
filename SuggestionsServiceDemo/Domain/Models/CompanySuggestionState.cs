namespace SuggestionsServiceDemo.Domain.Models;

/// <summary>
/// Potential states of suggested company partnerships.
/// </summary>
public enum CompanySuggestionState
{
    /// <summary>
    /// Suggestion has yet to be actioned.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Suggestion has been accepted.
    /// </summary>
    Accepted,

    /// <summary>
    /// Suggestion has been declined.
    /// </summary>
    Declined
}
