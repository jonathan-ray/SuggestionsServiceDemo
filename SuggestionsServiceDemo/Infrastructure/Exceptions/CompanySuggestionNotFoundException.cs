namespace SuggestionsServiceDemo.Infrastructure.Exceptions;

public class CompanySuggestionNotFoundException : Exception
{
    public CompanySuggestionNotFoundException(int companyId, int suggestedCompanyId)
    {
        this.CompanyId = companyId;
        this.SuggestedCompanyId = suggestedCompanyId;
    }

    public int CompanyId { get; private set; }

    public int SuggestedCompanyId { get; private set; }

    public override string Message => $"Suggested company ID '{this.SuggestedCompanyId}' not found for company ID '{this.CompanyId}'.";
}
