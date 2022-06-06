using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Domain.Adapters.Inbound;
using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Application.Ports;

public class CompaniesAdapter : ICompaniesAdapter
{
    private readonly ICompaniesOrchestrator companiesOrchestrator;
    private readonly IMailOrchestrator mailOrchestrator;
    private readonly IScheduleOrchestrator scheduleOrchestrator;

    public CompaniesAdapter(
        ICompaniesOrchestrator companiesOrchestrator,
        IMailOrchestrator mailOrchestrator,
        IScheduleOrchestrator scheduleOrchestrator)
    {
        this.companiesOrchestrator = companiesOrchestrator ?? throw new ArgumentNullException(nameof(companiesOrchestrator));
        this.mailOrchestrator = mailOrchestrator ?? throw new ArgumentNullException(nameof(mailOrchestrator));
        this.scheduleOrchestrator = scheduleOrchestrator ?? throw new ArgumentNullException(nameof(scheduleOrchestrator));
    }

    public async Task CompanyCreated(Company company)
    {
        if (company is null)
        {
            throw new ArgumentNullException(nameof(company));
        }

        await this.companiesOrchestrator.GenerateCompanySuggestions(company);

        var mailSequence = await this.mailOrchestrator.GenerateMailSequence(company);

        await this.scheduleOrchestrator.ScheduleMail(company.Id, mailSequence);
    }
}
