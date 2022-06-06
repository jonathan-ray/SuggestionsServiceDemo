using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Domain.Adapters.Outbound;

/// <summary>
/// Abstraction of the outbound calls to the Companies Service
/// </summary>
public interface ICompaniesService
{
    /// <summary>
    /// Retrieves companies based on their country and industry.
    /// </summary>
    /// <param name="country">Requested company country.</param>
    /// <param name="industry">Requested company industry.</param>
    /// <returns>A collection of matching companies.</returns>
    Task<IReadOnlyList<Company>> GetCompanies(string country, string industry);
}
