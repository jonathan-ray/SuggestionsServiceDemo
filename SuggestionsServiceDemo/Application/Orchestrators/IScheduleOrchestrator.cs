using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Application.Orchestrators;

/// <summary>
/// Orchestrates schedule-related workflows.
/// </summary>
public interface IScheduleOrchestrator
{
    /// <summary>
    /// Schedules a mail to be sent in the future for a given company.
    /// </summary>
    /// <param name="companyId">The ID of the given company.</param>
    /// <param name="mailSequence">The sequence of scheduled mail details.</param>
    /// <param name="lastMailTypeId">The last mail type that was sent (if any sent at all).</param>
    Task ScheduleMail(int companyId, IReadOnlyList<ScheduledMailDetails> mailSequence, int? lastMailTypeId = null);
}
