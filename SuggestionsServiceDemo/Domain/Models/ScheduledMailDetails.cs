namespace SuggestionsServiceDemo.Domain.Models;

/// <summary>
/// Overarching details of a scheduled mail to be sent.
/// </summary>
/// <param name="MailTypeId">The type of mail to be sent.</param>
/// <param name="DelayToSend">The time delay since the last sent mail before this should be sent.</param>
public record ScheduledMailDetails(int MailTypeId, TimeSpan DelayToSend);
