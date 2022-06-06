using SuggestionsServiceDemo.Domain.Adapters.Outbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Application.Orchestrators;

public class ScheduleOrchestrator : IScheduleOrchestrator
{
    private readonly ITimerService timerService;

    public ScheduleOrchestrator(ITimerService timerService)
    {
        this.timerService = timerService ?? throw new ArgumentNullException(nameof(timerService));
    }

    public async Task ScheduleMail(int companyId, IReadOnlyList<ScheduledMailDetails> mailSequence, int? lastMailTypeId = null)
    {
        if (mailSequence.Count == 0)
        {
            throw new MailSequenceUnavailableException(companyId);
        }

        ScheduledMailDetails mailToSchedule;
        if (lastMailTypeId is null)
        {
            mailToSchedule = mailSequence.First();
        }
        else
        {
            var lastScheduledMailIndex = mailSequence.ToList().FindIndex(details => details.MailTypeId == lastMailTypeId.Value);
            if (lastScheduledMailIndex < 0)
            {
                throw new UnknownMailTypeException(companyId, lastMailTypeId.Value);
            }

            var nextScheduledMailIndex = lastScheduledMailIndex + 1;
            if (nextScheduledMailIndex >= mailSequence.Count)
            {
                // No more scheduled mails left in the sequence; nothing else to do.
                return;
            }

            mailToSchedule = mailSequence[nextScheduledMailIndex];
        }

        await this.timerService.ScheduleMailNotification(companyId, mailToSchedule.MailTypeId, mailToSchedule.DelayToSend);
    }
}
