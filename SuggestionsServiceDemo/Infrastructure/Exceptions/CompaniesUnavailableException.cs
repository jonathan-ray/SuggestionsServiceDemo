namespace SuggestionsServiceDemo.Infrastructure.Exceptions;

public class CompaniesUnavailableException : Exception
{
    public CompaniesUnavailableException(string country, string industry)
    {
        this.Country = country;
        this.Industry = industry;
    }

    public string Country { get; private set; }

    public string Industry { get; private set; }

    public override string Message => $"Retrieved zero companies based in country '{this.Country}' and industry '{this.Industry}'.";
}
