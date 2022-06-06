using SuggestionsServiceDemo.Domain.Adapters.Outbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Application.Orchestrators;

public class MailOrchestrator : IMailOrchestrator
{
    private readonly IGrowthPolicyService growthPolicyService;
    private readonly IMailerService mailerService;
    private readonly IPersistenceRepository persistenceRepository;

    public MailOrchestrator(
        IGrowthPolicyService growthPolicyService,
        IMailerService mailerService,
        IPersistenceRepository persistenceRepository)
    {
        this.growthPolicyService = growthPolicyService ?? throw new ArgumentNullException(nameof(growthPolicyService));
        this.mailerService = mailerService ?? throw new ArgumentNullException(nameof(mailerService));
        this.persistenceRepository = persistenceRepository ?? throw new ArgumentNullException(nameof(persistenceRepository));
    }

    public async Task<IReadOnlyList<ScheduledMailDetails>> GenerateMailSequence(Company company)
    {
        var mailSequence = await this.growthPolicyService.GetScheduledMailSequence(company.Id);

        if (mailSequence.Count == 0)
        {
            throw new MailSequenceUnavailableException(company.Id);
        }

        await this.persistenceRepository.StoreMailSequence(company.Id, mailSequence);

        return mailSequence;
    }

    public async Task<IReadOnlyList<ScheduledMailDetails>> GetMailSequence(int companyId)
    {
        var mailSequence = await this.persistenceRepository.GetMailSequence(companyId);

        if (mailSequence is null || mailSequence.Count == 0)
        {
            throw new MailSequenceNotFoundException(companyId);
        }

        return mailSequence;
    }

    public async Task SendPendingSuggestionsMail(int companyId, int mailTypeId, IReadOnlyList<CompanySuggestion> pendingSuggestedCompanies)
    {
        var groupMailItem = await this.growthPolicyService.CreatePendingSuggestionsMail(
            companyId, 
            mailTypeId,
            pendingSuggestedCompanies.Select(suggestion => suggestion.CompanyId).ToList());

        await this.mailerService.SendMail(groupMailItem.Title, groupMailItem.Content, groupMailItem.RecipientEmails);
    }
}
