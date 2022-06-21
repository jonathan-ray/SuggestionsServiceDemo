using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Domain.Adapters.Outbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Tests.UnitTests.Application.Orchestrators;

[Trait("Category", "UnitTests")]
public class MailOrchestratorUnitTests
{
    private readonly Mock<IGrowthPolicyService> growthPolicyServiceMock;
    private readonly Mock<IMailerService> mailerServiceMock;
    private readonly Mock<IPersistenceRepository> persistenceRepositoryMock;

    private readonly IMailOrchestrator orchestratorUnderTest;

    public MailOrchestratorUnitTests()
    {
        this.growthPolicyServiceMock = new Mock<IGrowthPolicyService>();
        this.mailerServiceMock = new Mock<IMailerService>();
        this.persistenceRepositoryMock = new Mock<IPersistenceRepository>();

        this.orchestratorUnderTest = new MailOrchestrator(
            this.growthPolicyServiceMock.Object,
            this.mailerServiceMock.Object,
            this.persistenceRepositoryMock.Object);
    }

    [Fact]
    public void Construction_WithNullGrowthPolicyService_ShouldThrowException()
    {
        var construction = () => new MailOrchestrator(
            growthPolicyService: null!,
            Mock.Of<IMailerService>(),
            Mock.Of<IPersistenceRepository>());

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("growthPolicyService");
    }

    [Fact]
    public void Construction_WithNullMailerService_ShouldThrowException()
    {
        var construction = () => new MailOrchestrator(
            Mock.Of<IGrowthPolicyService>(),
            mailerService: null!,
            Mock.Of<IPersistenceRepository>());

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("mailerService");
    }

    [Fact]
    public void Construction_WithNullPersistenceRepository_ShouldThrowException()
    {
        var construction = () => new MailOrchestrator(
            Mock.Of<IGrowthPolicyService>(),
            Mock.Of<IMailerService>(),
            persistenceRepository: null!);

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("persistenceRepository");
    }

    [Fact]
    public async Task GenerateMailSequence_WithValidData_ShouldReturnGeneratedMailSequence()
    {
        var company = new Company(414, "Andorra", "Music");

        var expectedMailSequence = new List<ScheduledMailDetails>
        {
            new(123, TimeSpan.Zero)
        };

        this.growthPolicyServiceMock
            .Setup(m => m.GetScheduledMailSequence(company.Id))
            .ReturnsAsync(expectedMailSequence);

        var actualMailSequence = await this.orchestratorUnderTest.GenerateMailSequence(company);

        actualMailSequence.Should().NotBeNull();
        actualMailSequence.Should().BeSameAs(expectedMailSequence);

        this.persistenceRepositoryMock.Verify(m =>
            m.StoreMailSequence(company.Id, expectedMailSequence),
            Times.Once);
    }

    [Fact]
    public async Task GenerateMailSequence_WithEmptyDataReturned_ShouldThrowException()
    {
        var company = new Company(123, "Andorra", "Music");

        this.growthPolicyServiceMock
            .Setup(m => m.GetScheduledMailSequence(company.Id))
            .ReturnsAsync(new List<ScheduledMailDetails>());

        await this.orchestratorUnderTest
            .Awaiting(a => a.GenerateMailSequence(company))
            .Should().ThrowAsync<MailSequenceUnavailableException>();

        this.persistenceRepositoryMock.Verify(m =>
            m.StoreMailSequence(It.IsAny<int>(), It.IsAny<IReadOnlyList<ScheduledMailDetails>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetMailSequence_WithPersistedData_ShouldReturnData()
    {
        const int companyId = 442;

        var expectedMailSequence = new List<ScheduledMailDetails>
        {
            new(444, TimeSpan.Zero)
        };

        this.persistenceRepositoryMock
            .Setup(m => m.GetMailSequence(companyId))
            .ReturnsAsync(expectedMailSequence);

        var actualMailSequence = await this.orchestratorUnderTest.GetMailSequence(companyId);

        actualMailSequence.Should().NotBeNull();
        actualMailSequence.Should().BeSameAs(expectedMailSequence);
    }

    [Fact]
    public async Task GetMailSequence_WithEmptyData_ShouldThrowException()
    {
        const int companyId = 442;

        this.persistenceRepositoryMock
            .Setup(m => m.GetMailSequence(companyId))
            .ReturnsAsync(new List<ScheduledMailDetails>());

        await this.orchestratorUnderTest
            .Awaiting(o => o.GetMailSequence(companyId))
            .Should().ThrowAsync<MailSequenceNotFoundException>();
    }

    [Fact]
    public async Task GetMailSequence_WithNullData_ShouldThrowException()
    {
        const int companyId = 442;

        this.persistenceRepositoryMock
            .Setup(m => m.GetMailSequence(companyId))
            .ReturnsAsync((IReadOnlyList<ScheduledMailDetails>?)null);

        await this.orchestratorUnderTest
            .Awaiting(o => o.GetMailSequence(companyId))
            .Should().ThrowAsync<MailSequenceNotFoundException>();
    }

    [Fact]
    public async Task SendPendingSuggestionsMail_WithValidData_ShouldSendSuccessfully()
    {
        const int companyId = 214;
        const int mailTypeId = 5;
        var pendingCompanies = new List<CompanySuggestion> { new(1225) };

        var expectedMailItem = new GroupMailItem(
            "Title of mail",
            "The content of the mail.",
            new[] { "joe@bloggs.com", "maira@dias.com" });

        this.growthPolicyServiceMock
            .Setup(m => 
                m.CreatePendingSuggestionsMail(
                    companyId,
                    mailTypeId,
                    It.Is<IReadOnlyList<int>>(ids => ids.Count == 1 && ids[0] == pendingCompanies[0].CompanyId)))
            .ReturnsAsync(expectedMailItem);

        await this.orchestratorUnderTest.SendPendingSuggestionsMail(companyId, mailTypeId, pendingCompanies);

        this.mailerServiceMock.Verify(m =>
            m.SendMail(expectedMailItem.Title, expectedMailItem.Content, expectedMailItem.RecipientEmails),
            Times.Once);
    }
}
