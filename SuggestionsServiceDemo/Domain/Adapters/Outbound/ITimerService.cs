namespace SuggestionsServiceDemo.Domain.Adapters.Outbound;

/// <summary>
/// Abstraction of outbound calls to the Timer Service.
/// </summary>
public interface ITimerService
{
    /// <summary>
    /// Schedules a notification for when to send a specific type of mail.
    /// </summary>
    /// <param name="companyId">The ID of the company to send the mail to.</param>
    /// <param name="mailTypeId">The type of mail.</param>
    /// <param name="scheduledDelay">The delay until the mail notification should be sent.</param>
    Task ScheduleMailNotification(int companyId, int mailTypeId, TimeSpan scheduledDelay);
}
