using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Application.Ports;
using SuggestionsServiceDemo.Domain.Adapters.Inbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Tests.UnitTests.Application.Ports;

[Trait("Category", "UnitTests")]
public class WebAdapterUnitTests
{
    private readonly Mock<ICompaniesOrchestrator> companiesOrchestratorMock;

    private readonly IWebAdapter adapterUnderTest;

    public WebAdapterUnitTests()
    {
        this.companiesOrchestratorMock = new Mock<ICompaniesOrchestrator>();

        this.adapterUnderTest = new WebAdapter(this.companiesOrchestratorMock.Object);
    }

    [Fact]
    public void Construction_WithNullCompaniesOrchestrator_ShouldThrowException()
    {
        var construction = () => new WebAdapter(companiesOrchestrator: null!);

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("companiesOrchestrator");
    }

    [Theory]
    [InlineData(CompanySuggestionState.Accepted)]
    [InlineData(CompanySuggestionState.Declined)]
    public async Task UpdateCompanySuggestionState_WithValidInput_ShouldUpdateSuggestionState(CompanySuggestionState updatedState)
    {
        const int companyId = 123;
        const int suggestedCompanyId = 789;

        var companySuggestion = new CompanySuggestion(suggestedCompanyId)
        {
            State = CompanySuggestionState.Pending
        };

        this.companiesOrchestratorMock
            .Setup(m => m.GetCompanySuggestion(companyId, suggestedCompanyId))
            .ReturnsAsync(companySuggestion);

        await this.adapterUnderTest.UpdateCompanySuggestionState(companyId, suggestedCompanyId, updatedState);

        this.companiesOrchestratorMock.Verify(m =>
            m.UpdateCompanySuggestion(
                companyId,
                It.Is<CompanySuggestion>(suggestion => suggestion.CompanyId == suggestedCompanyId && suggestion.State == updatedState)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCompanySuggestionState_WithNotSupportedState_ShouldThrowException()
    {
        const int companyId = 123;
        const int suggestedCompanyId = 789;
        const CompanySuggestionState updatedState = CompanySuggestionState.Pending;

        await this.adapterUnderTest
            .Awaiting(a => a.UpdateCompanySuggestionState(companyId, suggestedCompanyId, updatedState))
            .Should().ThrowAsync<UnsupportedCompanySuggestionStateUpdateException>();

        this.companiesOrchestratorMock.Verify(m =>
            m.UpdateCompanySuggestion(
                It.IsAny<int>(),
                It.IsAny<CompanySuggestion>()),
            Times.Never);
    }
}
