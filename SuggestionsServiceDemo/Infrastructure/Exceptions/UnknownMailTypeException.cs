namespace SuggestionsServiceDemo.Infrastructure.Exceptions;

public class UnknownMailTypeException : Exception
{
    public UnknownMailTypeException(int companyId, int mailTypeId)
    {
        this.CompanyId = companyId;
        this.MailTypeId = mailTypeId;
    }

    public int CompanyId { get; private set; }

    public int MailTypeId { get; private set; }

    public override string Message => $"Unknown mail type '{this.MailTypeId}' scheduled for company '{this.CompanyId}'.";
}
