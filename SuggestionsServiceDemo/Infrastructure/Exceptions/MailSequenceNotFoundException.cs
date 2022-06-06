namespace SuggestionsServiceDemo.Infrastructure.Exceptions;

public class MailSequenceNotFoundException : Exception
{
    public MailSequenceNotFoundException(int companyId)
    {
        this.CompanyId = companyId;
    }

    public int CompanyId { get; private set; }

    public override string Message => $"Could not find any persisted mail sequence data for company '{this.CompanyId}'.";
}
