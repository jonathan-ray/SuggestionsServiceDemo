using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Domain.Adapters.Inbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Application.Ports;

public class WebAdapter : IWebAdapter
{
    private readonly ICompaniesOrchestrator companiesOrchestrator;

    public WebAdapter(ICompaniesOrchestrator companiesOrchestrator)
    {
        this.companiesOrchestrator = companiesOrchestrator ?? throw new ArgumentNullException(nameof(companiesOrchestrator));
    }

    public async Task UpdateCompanySuggestionState(int companyId, int suggestedCompanyId, CompanySuggestionState updatedState)
    {
        // Verify the proposed state is one we support
        if (updatedState != CompanySuggestionState.Accepted && updatedState != CompanySuggestionState.Declined)
        {
            throw new UnsupportedCompanySuggestionStateUpdateException(updatedState);
        }

        var companySuggestionToUpdate = await this.companiesOrchestrator.GetCompanySuggestion(companyId, suggestedCompanyId);

        companySuggestionToUpdate.State = updatedState;

        await this.companiesOrchestrator.UpdateCompanySuggestion(companyId, companySuggestionToUpdate);
    }
}
