using SuggestionsServiceDemo.Domain.Adapters.Outbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Application.Orchestrators;

public class CompaniesOrchestrator : ICompaniesOrchestrator
{
    private readonly ICompaniesService companiesService;
    private readonly IPersistenceRepository persistenceRepository;

    public CompaniesOrchestrator(
        ICompaniesService companiesService,
        IPersistenceRepository persistenceRepository)
    {
        this.companiesService = companiesService ?? throw new ArgumentNullException(nameof(companiesService));
        this.persistenceRepository = persistenceRepository ?? throw new ArgumentNullException(nameof(persistenceRepository));
    }

    public async Task GenerateCompanySuggestions(Company company)
    {
        var relatedCompanies = await this.companiesService.GetCompanies(company.Country, company.Industry);

        var validSuggestedCompanies = FilterValidCompanySuggestions(company, relatedCompanies);
        if (validSuggestedCompanies.Count == 0)
        {
            throw new CompaniesUnavailableException(company.Country, company.Industry);
        }

        var companySuggestions = CreateCompanySuggestions(validSuggestedCompanies);

        await this.persistenceRepository.StoreCompanySuggestions(company.Id, companySuggestions);
    }

    public async Task<IReadOnlyList<CompanySuggestion>> GetAllCompanySuggestions(int companyId, Predicate<CompanySuggestion>? filter = null)
    {
        var companySuggestions = await this.persistenceRepository.GetCompanySuggestions(companyId);
        if (companySuggestions is null || companySuggestions.Count == 0)
        {
            throw new CompanyNotFoundException(companyId);
        }

        if (filter is not null)
        {
            companySuggestions = companySuggestions
                .Where(suggestion => filter(suggestion))
                .ToList();
        }

        return companySuggestions;
    }

    public async Task<CompanySuggestion> GetCompanySuggestion(int companyId, int suggestedCompanyId)
    {
        var companySuggestions = await this.GetAllCompanySuggestions(companyId);

        var companySuggestionToUpdate = companySuggestions.FirstOrDefault(suggestion => suggestion.CompanyId == suggestedCompanyId);
        if (companySuggestionToUpdate is null)
        {
            throw new CompanySuggestionNotFoundException(companyId, suggestedCompanyId);
        }

        return companySuggestionToUpdate;
    }

    public Task UpdateCompanySuggestion(int companyId, CompanySuggestion updatedCompanySuggestion)
    {
        return this.persistenceRepository.UpdateCompanySuggestion(companyId, updatedCompanySuggestion);
    }

    private static IReadOnlyList<Company> FilterValidCompanySuggestions(Company newCompany, IReadOnlyList<Company> companySuggestions)
    {
        // In case it was sent by the companies service, filter out the new company in case that has been returned also.
        // In the future, we can also filter to a set number of suggestions, or potentially discard ones that are less favourable to suggest.
        return companySuggestions
            .Where(suggestedCompany => suggestedCompany.Id != newCompany.Id)
            .ToList();
    }

    private static IReadOnlyList<CompanySuggestion> CreateCompanySuggestions(IReadOnlyList<Company> companies)
    {
        return companies
            .Select(company => new CompanySuggestion(company.Id))
            .ToList();
    }
}
