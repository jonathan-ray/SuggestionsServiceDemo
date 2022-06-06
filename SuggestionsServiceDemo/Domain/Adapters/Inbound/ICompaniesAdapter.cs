using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Domain.Adapters.Inbound;

/// <summary>
/// Abstraction of the inbound calls from the Companies Service.
/// </summary>
public interface ICompaniesAdapter
{
    /// <summary>
    /// Takes a newly created company and processes it within the suggestions service.
    /// </summary>
    /// <param name="company">The new company.</param>
    Task CompanyCreated(Company company);
}
