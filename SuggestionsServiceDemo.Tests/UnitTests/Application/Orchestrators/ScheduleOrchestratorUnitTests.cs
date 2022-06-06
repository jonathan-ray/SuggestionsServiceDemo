using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Domain.Adapters.Outbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Tests.UnitTests.Application.Orchestrators;

[Trait("Category", "UnitTests")]
public class ScheduleOrchestratorUnitTests
{
    private readonly Mock<ITimerService> timerServiceMock;

    private IScheduleOrchestrator orchestratorUnderTest;

    public ScheduleOrchestratorUnitTests()
    {
        this.timerServiceMock = new Mock<ITimerService>();

        this.orchestratorUnderTest = new ScheduleOrchestrator(this.timerServiceMock.Object);
    }

    [Fact]
    public void Construction_WithNullTimerService_ShouldThrowException()
    {
        Func<ScheduleOrchestrator> construction = () => new ScheduleOrchestrator(timerService: null!);

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("timerService");
    }

    [Fact]
    public async Task ScheduleMail_WithMailTypeKnownAndNotLastInSequence_ShouldScheduleMail()
    {
        const int companyId = 545;
        const int lastMailTypeId = 31;
        var expectedMailDetails = new ScheduledMailDetails(41, TimeSpan.FromMinutes(15));

        var mailSequence = new List<ScheduledMailDetails>
        {
            new(lastMailTypeId, TimeSpan.Zero),
            expectedMailDetails,
            new(51, TimeSpan.FromMinutes(50)),
            new(71, TimeSpan.FromMinutes(2300)),
        };

        await this.orchestratorUnderTest.ScheduleMail(companyId, mailSequence, lastMailTypeId);

        this.timerServiceMock.Verify(m =>
            m.ScheduleMailNotification(companyId, expectedMailDetails.MailTypeId, expectedMailDetails.DelayToSend),
            Times.Once);
    }

    [Fact]
    public async Task ScheduleMail_WithMailTypeKnownAndLastInSequence_ShouldDoNothing()
    {
        const int companyId = 545;
        const int lastMailTypeId = 31;

        var mailSequence = new List<ScheduledMailDetails>
        {
            new(51, TimeSpan.FromMinutes(50)),
            new(71, TimeSpan.FromMinutes(150)),
            new(lastMailTypeId, TimeSpan.FromMinutes(2300)),
        };

        await this.orchestratorUnderTest.ScheduleMail(companyId, mailSequence, lastMailTypeId);

        this.timerServiceMock.Verify(m =>
            m.ScheduleMailNotification(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }

    [Fact]
    public async Task ScheduleMail_WithMailTypeGivenButUnknown_ShouldThrowException()
    {
        const int companyId = 545;
        const int lastMailTypeId = 31;

        var mailSequence = new List<ScheduledMailDetails>
        {
            new(52, TimeSpan.FromMinutes(50)),
            new(72, TimeSpan.FromMinutes(150)),
            new(82, TimeSpan.FromMinutes(2300)),
        };

        await this.orchestratorUnderTest
            .Awaiting(o => o.ScheduleMail(companyId, mailSequence, lastMailTypeId))
            .Should().ThrowAsync<UnknownMailTypeException>();

        this.timerServiceMock.Verify(m =>
            m.ScheduleMailNotification(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }

    [Fact]
    public async Task ScheduleMail_WithEmptyMailSequence_ShouldThrowException()
    {
        const int companyId = 545;
        const int lastMailTypeId = 31;

        var mailSequence = new List<ScheduledMailDetails>();

        await this.orchestratorUnderTest
            .Awaiting(o => o.ScheduleMail(companyId, mailSequence, lastMailTypeId))
            .Should().ThrowAsync<MailSequenceUnavailableException>();

        this.timerServiceMock.Verify(m =>
            m.ScheduleMailNotification(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>()),
            Times.Never);
    }

    [Fact]
    public async Task ScheduleMail_WithMailTypeNotGiven_ShouldScheduleFirstMail()
    {
        const int companyId = 545;
        var expectedMailDetails = new ScheduledMailDetails(41, TimeSpan.FromMinutes(15));

        var mailSequence = new List<ScheduledMailDetails>
        {
            expectedMailDetails,
            new(51, TimeSpan.FromMinutes(50)),
            new(71, TimeSpan.FromMinutes(2300)),
        };

        await this.orchestratorUnderTest.ScheduleMail(companyId, mailSequence, null);

        this.timerServiceMock.Verify(m =>
            m.ScheduleMailNotification(companyId, expectedMailDetails.MailTypeId, expectedMailDetails.DelayToSend),
            Times.Once);
    }
}
