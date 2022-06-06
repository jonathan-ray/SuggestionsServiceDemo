namespace SuggestionsServiceDemo.Infrastructure.Exceptions;

public class CompanyNotFoundException : Exception
{
    public CompanyNotFoundException(int companyId)
    {
        this.CompanyId = companyId;
    }

    public int CompanyId { get; private set; }

    public override string Message => $"Unable to find a record of company with ID '{this.CompanyId}'.";
}
