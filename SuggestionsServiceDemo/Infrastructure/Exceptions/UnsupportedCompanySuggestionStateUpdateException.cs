using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Infrastructure.Exceptions;

public class UnsupportedCompanySuggestionStateUpdateException : Exception
{
    public UnsupportedCompanySuggestionStateUpdateException(CompanySuggestionState suggestionState)
    {
        this.SuggestionState = suggestionState;
    }

    public CompanySuggestionState SuggestionState { get; private set; }

    public override string Message => $"Updating to suggestion state '{this.SuggestionState} is not supported.";
}
