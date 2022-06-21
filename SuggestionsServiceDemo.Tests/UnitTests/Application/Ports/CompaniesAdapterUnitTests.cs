using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Application.Ports;
using SuggestionsServiceDemo.Domain.Adapters.Inbound;
using SuggestionsServiceDemo.Domain.Models;

namespace SuggestionsServiceDemo.Tests.UnitTests.Application.Ports;

[Trait("Category", "UnitTests")]
public class CompaniesAdapterUnitTests
{

    private readonly Mock<ICompaniesOrchestrator> companiesOrchestratorMock;
    private readonly Mock<IMailOrchestrator> mailOrchestratorMock;
    private readonly Mock<IScheduleOrchestrator> scheduleOrchestratorMock;

    private readonly ICompaniesAdapter adapterUnderTest;

    public CompaniesAdapterUnitTests()
    {
        this.companiesOrchestratorMock = new Mock<ICompaniesOrchestrator>();
        this.mailOrchestratorMock = new Mock<IMailOrchestrator>();
        this.scheduleOrchestratorMock = new Mock<IScheduleOrchestrator>();

        this.adapterUnderTest = new CompaniesAdapter(
            this.companiesOrchestratorMock.Object,
            this.mailOrchestratorMock.Object,
            this.scheduleOrchestratorMock.Object);
    }

    [Fact]
    public void Construction_WithNullCompaniesOrchestrator_ShouldThrowException()
    {
        var construction = () => new CompaniesAdapter(
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
        var construction = () => new CompaniesAdapter(
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
        var construction = () => new CompaniesAdapter(
            Mock.Of<ICompaniesOrchestrator>(),
            Mock.Of<IMailOrchestrator>(),
            scheduleOrchestrator: null!);

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("scheduleOrchestrator");
    }

    [Fact]
    public async Task CompanyCreated_WithValidData_ShouldReturnSuccessfully()
    {
        const int companyId = 456;
        const string country = "Brazil";
        const string industry = "Gaming";

        var newCompany = new Company(companyId, country, industry);

        var mailSequence = new List<ScheduledMailDetails> { new(11, TimeSpan.Zero) };

        this.mailOrchestratorMock
            .Setup(m => m.GenerateMailSequence(newCompany))
            .ReturnsAsync(mailSequence);

        await this.adapterUnderTest.CompanyCreated(newCompany);

        this.companiesOrchestratorMock.Verify(m =>
            m.GenerateCompanySuggestions(newCompany),
            Times.Once);

        this.scheduleOrchestratorMock.Verify(m =>
            m.ScheduleMail(companyId, mailSequence, null),
            Times.Once);
    }

    [Fact]
    public async Task CompanyCreated_WithNullCompany_ShouldThrowException()
    {
        await this.adapterUnderTest
            .Awaiting(a => a.CompanyCreated(null!))
            .Should().ThrowAsync<ArgumentNullException>();

        this.companiesOrchestratorMock.Verify(m =>
            m.GenerateCompanySuggestions(It.IsAny<Company>()),
            Times.Never);

        this.scheduleOrchestratorMock.Verify(m =>
            m.ScheduleMail(It.IsAny<int>(), It.IsAny<IReadOnlyList<ScheduledMailDetails>>(), It.IsAny<int?>()),
            Times.Never);
    }
}
