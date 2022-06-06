namespace SuggestionsServiceDemo.Domain.Adapters.Inbound;

/// <summary>
/// Abstraction of inbound calls from the Timer Service.
/// </summary>
public interface ITimerAdapter
{
    /// <summary>
    /// On notification received to the suggestion service to send a scheduled mail.
    /// </summary>
    /// <param name="companyId">The ID of the company to send the mail to.</param>
    /// <param name="mailTypeId">The type of mail to send.</param>
    Task ScheduledMailNotificationReceived(int companyId, int mailTypeId);
}
