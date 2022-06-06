using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Application.Ports;
using SuggestionsServiceDemo.Domain.Adapters.Inbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Tests.UnitTests.Application.Ports;

[Trait("Category", "UnitTests")]
public class TimerAdapterUnitTests
{
    private readonly Mock<ICompaniesOrchestrator> companiesOrchestratorMock;
    private readonly Mock<IMailOrchestrator> mailOrchestratorMock;
    private readonly Mock<IScheduleOrchestrator> scheduleOrchestratorMock;

    private ITimerAdapter adapterUnderTest;

    public TimerAdapterUnitTests()
    {
        this.companiesOrchestratorMock = new Mock<ICompaniesOrchestrator>();
        this.mailOrchestratorMock = new Mock<IMailOrchestrator>();
        this.scheduleOrchestratorMock = new Mock<IScheduleOrchestrator>();

        this.adapterUnderTest = new TimerAdapter(
            this.companiesOrchestratorMock.Object,
            this.mailOrchestratorMock.Object,
            this.scheduleOrchestratorMock.Object);
    }

    [Fact]
    public void Construction_WithNullCompaniesOrchestrator_ShouldThrowException()
    {
        Func<TimerAdapter> construction = () => new TimerAdapter(
            companiesOrchestrator: null!,
            Mock.Of<IMailOrchestrator>(),
            Mock.Of<IScheduleOrchestrator>());

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("companiesOrchestrator");
    }

    [Fact]
    public void Construction_WithNullMailOrchestrator_ShouldThrowException()
    {
        Func<TimerAdapter> construction = () => new TimerAdapter(
            Mock.Of<ICompaniesOrchestrator>(),
            mailOrchestrator: null!,
            Mock.Of<IScheduleOrchestrator>());

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("mailOrchestrator");
    }

    [Fact]
    public void Construction_WithNullScheduleOrchestrator_ShouldThrowException()
    {
        Func<TimerAdapter> construction = () => new TimerAdapter(
            Mock.Of<ICompaniesOrchestrator>(),
            Mock.Of<IMailOrchestrator>(),
            scheduleOrchestrator: null!);

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("scheduleOrchestrator");
    }

    [Fact]
    public async Task ScheduledMailNotificationReceived_WithPendingSuggestedCompanies_ShouldReturnSuccessfully()
    {
        const int companyId = 345;
        const int mailTypeId = 12;

        var companySuggestions = new List<CompanySuggestion>
        {
            new(777),
            new(888),
            new(999)
        };

        this.companiesOrchestratorMock
            .Setup(m => m.GetAllCompanySuggestions(companyId, It.IsAny<Predicate<CompanySuggestion>>()))
            .ReturnsAsync(companySuggestions);

        var mailSequence = new List<ScheduledMailDetails>
        {
            new(mailTypeId, TimeSpan.FromMinutes(12))
        };

        this.mailOrchestratorMock
            .Setup(m => m.GetMailSequence(companyId))
            .ReturnsAsync(mailSequence);

        await this.adapterUnderTest.ScheduledMailNotificationReceived(companyId, mailTypeId);

        this.mailOrchestratorMock.Verify(m =>
            m.SendPendingSuggestionsMail(companyId, mailTypeId, companySuggestions),
            Times.Once);

        this.scheduleOrchestratorMock.Verify(m =>
            m.ScheduleMail(companyId, mailSequence, mailTypeId),
            Times.Once);
    }

    [Fact]
    public async Task ScheduledMailNotificationReceived_WithoutPendingSuggestedCompanies_ShouldReturnSuccessfully()
    {
        const int companyId = 345;
        const int mailTypeId = 12;

        var companySuggestions = new List<CompanySuggestion>();

        this.companiesOrchestratorMock
            .Setup(m => m.GetAllCompanySuggestions(companyId, It.IsAny<Predicate<CompanySuggestion>>()))
            .ReturnsAsync(companySuggestions);

        await this.adapterUnderTest.ScheduledMailNotificationReceived(companyId, mailTypeId);

        this.mailOrchestratorMock.Verify(m =>
            m.SendPendingSuggestionsMail(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IReadOnlyList<CompanySuggestion>>()),
            Times.Never);

        this.scheduleOrchestratorMock.Verify(m =>
            m.ScheduleMail(It.IsAny<int>(), It.IsAny<IReadOnlyList<ScheduledMailDetails>>(), It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task ScheduledMailNotificationReceived_WithUnknownMailType_ShouldThrowException()
    {
        const int companyId = 345;
        const int mailTypeId = 12;

        var companySuggestions = new List<CompanySuggestion>
        {
            new(777)
        };

        this.companiesOrchestratorMock
            .Setup(m => m.GetAllCompanySuggestions(companyId, It.IsAny<Predicate<CompanySuggestion>>()))
            .ReturnsAsync(companySuggestions);

        var mailSequence = new List<ScheduledMailDetails>
        {
            new(98, TimeSpan.FromMinutes(12))
        };

        this.mailOrchestratorMock
            .Setup(m => m.GetMailSequence(companyId))
            .ReturnsAsync(mailSequence);

        await this.adapterUnderTest
            .Awaiting(a => a.ScheduledMailNotificationReceived(companyId, mailTypeId))
            .Should().ThrowAsync<UnknownMailTypeException>();

        this.mailOrchestratorMock.Verify(m =>
            m.SendPendingSuggestionsMail(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IReadOnlyList<CompanySuggestion>>()),
            Times.Never);

        this.scheduleOrchestratorMock.Verify(m =>
            m.ScheduleMail(It.IsAny<int>(), It.IsAny<IReadOnlyList<ScheduledMailDetails>>(), It.IsAny<int>()),
            Times.Never);
    }
}
