namespace SuggestionsServiceDemo.Infrastructure.Exceptions;

public class MailSequenceUnavailableException : Exception
{
    public MailSequenceUnavailableException(int companyId)
    {
        this.CompanyId = companyId;
    }

    public int CompanyId { get; private set; }

    public override string Message => $"Retrieved zero scheduled mail details for '{this.CompanyId}'.";
}
