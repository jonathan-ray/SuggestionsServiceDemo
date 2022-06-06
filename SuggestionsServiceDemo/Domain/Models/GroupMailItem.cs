namespace SuggestionsServiceDemo.Domain.Models;

/// <summary>
/// A mail item to be sent to one or more recipients.
/// </summary>
/// <param name="Title">Title of the mail.</param>
/// <param name="Content">Contents of the mail.</param>
/// <param name="RecipientEmails">Collection of recipients to send the mail to.</param>
public record GroupMailItem(string Title, string Content, IReadOnlyList<string> RecipientEmails);
