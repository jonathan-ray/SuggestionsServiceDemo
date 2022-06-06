namespace SuggestionsServiceDemo.Domain.Models;

/// <summary>
/// Relevant details of a company.
/// </summary>
/// <param name="Id">Primary Key for the company.</param>
/// <param name="Country">The location of the company.</param>
/// <param name="Industry">The industry the company.</param>
public record Company(int Id, string Country, string Industry);
