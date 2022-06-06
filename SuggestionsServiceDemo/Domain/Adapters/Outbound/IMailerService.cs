namespace SuggestionsServiceDemo.Domain.Adapters.Outbound;

/// <summary>
/// Abstraction of outbound calls to the Mailer Service.
/// </summary>
public interface IMailerService
{
    /// <summary>
    /// Sends a mail to a collection of recipients.
    /// </summary>
    /// <param name="title">The title of the mail.</param>
    /// <param name="content">The content of the mail.</param>
    /// <param name="recipients">The recipients' emails.</param>
    Task SendMail(string title, string content, IReadOnlyList<string> recipients);
}
