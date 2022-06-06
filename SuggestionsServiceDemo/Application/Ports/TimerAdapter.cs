using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Domain.Adapters.Inbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Application.Ports;

public class TimerAdapter : ITimerAdapter
{
    private readonly ICompaniesOrchestrator companiesOrchestrator;
    private readonly IMailOrchestrator mailOrchestrator;
    private readonly IScheduleOrchestrator scheduleOrchestrator;

    public TimerAdapter(
        ICompaniesOrchestrator companiesOrchestrator,
        IMailOrchestrator mailOrchestrator,
        IScheduleOrchestrator scheduleOrchestrator)
    {
        this.companiesOrchestrator = companiesOrchestrator ?? throw new ArgumentNullException(nameof(companiesOrchestrator));
        this.mailOrchestrator = mailOrchestrator ?? throw new ArgumentNullException(nameof(mailOrchestrator));
        this.scheduleOrchestrator = scheduleOrchestrator ?? throw new ArgumentNullException(nameof(scheduleOrchestrator));
    }

    public async Task ScheduledMailNotificationReceived(int companyId, int mailTypeId)
    {
        var pendingSuggestedCompanies = await this.companiesOrchestrator.GetAllCompanySuggestions(
            companyId,
            suggestion => suggestion.State == CompanySuggestionState.Pending);

        if (pendingSuggestedCompanies.Count == 0)
        {
            // No more pending suggested companies remain; nothing else to do.
            return;
        }

        var mailSequence = await this.mailOrchestrator.GetMailSequence(companyId);

        if (!mailSequence.Any(mailDetails => mailDetails.MailTypeId == mailTypeId))
        {
            throw new UnknownMailTypeException(companyId, mailTypeId);
        }

        await this.mailOrchestrator.SendPendingSuggestionsMail(companyId, mailTypeId, pendingSuggestedCompanies);

        await this.scheduleOrchestrator.ScheduleMail(companyId, mailSequence, mailTypeId);
    }
}
