using SuggestionsServiceDemo.Application.Orchestrators;
using SuggestionsServiceDemo.Domain.Adapters.Outbound;
using SuggestionsServiceDemo.Domain.Models;
using SuggestionsServiceDemo.Infrastructure.Exceptions;

namespace SuggestionsServiceDemo.Tests.UnitTests.Application.Orchestrators;

[Trait("Category", "UnitTests")]
public class CompaniesOrchestratorUnitTests
{
    private readonly Mock<ICompaniesService> companiesServiceMock;
    private readonly Mock<IPersistenceRepository> persistenceRepositoryMock;

    private readonly ICompaniesOrchestrator orchestratorUnderTest;

    public CompaniesOrchestratorUnitTests()
    {
        this.companiesServiceMock = new Mock<ICompaniesService>();
        this.persistenceRepositoryMock = new Mock<IPersistenceRepository>();

        this.orchestratorUnderTest = new CompaniesOrchestrator(
            this.companiesServiceMock.Object,
            this.persistenceRepositoryMock.Object);
    }

    [Fact]
    public void Construction_WithNullCompaniesService_ShouldThrowException()
    {
        var construction = () => new CompaniesOrchestrator(
            companiesService: null!,
            Mock.Of<IPersistenceRepository>());

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("companiesService");
    }

    [Fact]
    public void Construction_WithNullPersistenceRepository_ShouldThrowException()
    {
        var construction = () => new CompaniesOrchestrator(
            Mock.Of<ICompaniesService>(),
            persistenceRepository: null!);

        construction
            .Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("persistenceRepository");
    }

    [Fact]
    public async Task GenerateCompanySuggestions_WithValidData_ShouldCreateAndPersistSuccessfully()
    {
        var newCompany = new Company(1601, "Zambia", "Electronics");

        var expectedSuggestions = new List<Company>
        { 
            newCompany with { Id = 1220 },
            newCompany
        };

        this.companiesServiceMock
            .Setup(m => m.GetCompanies(newCompany.Country, newCompany.Industry))
            .ReturnsAsync(expectedSuggestions);

        await this.orchestratorUnderTest.GenerateCompanySuggestions(newCompany);

        this.persistenceRepositoryMock.Verify(m =>
            m.StoreCompanySuggestions(
                newCompany.Id,
                It.Is<IReadOnlyList<CompanySuggestion>>(suggestions =>
                    suggestions.Count == 1 &&
                    suggestions[0].CompanyId == expectedSuggestions[0].Id &&
                    suggestions[0].State == CompanySuggestionState.Pending)),
            Times.Once);
    }

    [Fact]
    public async Task GenerateCompanySuggestions_WithOnlyCallingCompanyData_ShouldThrowException()
    {
        var newCompany = new Company(1601, "Zambia", "Electronics");

        var suggestions = new List<Company> { newCompany };

        this.companiesServiceMock
            .Setup(m => m.GetCompanies(newCompany.Country, newCompany.Industry))
            .ReturnsAsync(suggestions);

        await this.orchestratorUnderTest
            .Awaiting(o => o.GenerateCompanySuggestions(newCompany))
            .Should().ThrowAsync<CompaniesUnavailableException>();

        this.persistenceRepositoryMock.Verify(m =>
            m.StoreCompanySuggestions(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyList<CompanySuggestion>>()),
            Times.Never);
    }

    [Fact]
    public async Task GenerateCompanySuggestions_WithEmptyData_ShouldThrowException()
    {
        var newCompany = new Company(1601, "Zambia", "Electronics");

        this.companiesServiceMock
            .Setup(m => m.GetCompanies(newCompany.Country, newCompany.Industry))
            .ReturnsAsync(new List<Company>());

        await this.orchestratorUnderTest
            .Awaiting(o => o.GenerateCompanySuggestions(newCompany))
            .Should().ThrowAsync<CompaniesUnavailableException>();

        this.persistenceRepositoryMock.Verify(m =>
            m.StoreCompanySuggestions(
                It.IsAny<int>(),
                It.IsAny<IReadOnlyList<CompanySuggestion>>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllCompanySuggestions_WithoutFilter_ShouldReturnAllData()
    {
        const int companyId = 9610;

        var expectedSuggestions = new List<CompanySuggestion>
        {
            new(11000),
            new(21122),
            new(72819)
        };

        this.persistenceRepositoryMock
            .Setup(m => m.GetCompanySuggestions(companyId))
            .ReturnsAsync(expectedSuggestions);

        var actualSuggestions = await this.orchestratorUnderTest.GetAllCompanySuggestions(companyId, null);

        actualSuggestions.Should().NotBeNull();
        actualSuggestions.Should().BeSameAs(expectedSuggestions);
    }

    [Fact]
    public async Task GetAllCompanySuggestions_WithFilter_ShouldReturnOnlyValidData()
    {
        const int companyId = 9610;

        var expectedSuggestions = new List<CompanySuggestion>
        {
            new(11000) { State = CompanySuggestionState.Accepted },
            new(21122) { State = CompanySuggestionState.Accepted },
            new(72819) { State = CompanySuggestionState.Declined }
        };

        this.persistenceRepositoryMock
            .Setup(m => m.GetCompanySuggestions(companyId))
            .ReturnsAsync(expectedSuggestions);

        var actualSuggestions = await this.orchestratorUnderTest.GetAllCompanySuggestions(
            companyId,
            suggestion => suggestion.State == CompanySuggestionState.Accepted);

        actualSuggestions.Should().NotBeNull();
        actualSuggestions.Should().BeEquivalentTo(expectedSuggestions.Where(suggestion => suggestion.State == CompanySuggestionState.Accepted));
    }

    [Fact]
    public async Task GetAllCompanySuggestions_WithEmptyData_ShouldThrowException()
    {
        const int companyId = 9610;

        this.persistenceRepositoryMock
            .Setup(m => m.GetCompanySuggestions(companyId))
            .ReturnsAsync(new List<CompanySuggestion>());

        await this.orchestratorUnderTest
            .Awaiting(o => o.GetAllCompanySuggestions(companyId, null))
            .Should().ThrowAsync<CompanyNotFoundException>();
    }

    [Fact]
    public async Task GetAllCompanySuggestions_WithNullData_ShouldThrowException()
    {
        const int companyId = 9610;

        this.persistenceRepositoryMock
            .Setup(m => m.GetCompanySuggestions(companyId))
            .ReturnsAsync((IReadOnlyList<CompanySuggestion>?)null);

        await this.orchestratorUnderTest
            .Awaiting(o => o.GetAllCompanySuggestions(companyId, null))
            .Should().ThrowAsync<CompanyNotFoundException>();
    }

    [Fact]
    public async Task GetAllCompanySuggestions_WithFilteringAllData_ShouldReturnEmpty()
    {
        const int companyId = 9610;

        var expectedSuggestions = new List<CompanySuggestion>
        {
            new(11000) { State = CompanySuggestionState.Accepted },
            new(21122) { State = CompanySuggestionState.Accepted },
            new(72819) { State = CompanySuggestionState.Declined }
        };

        this.persistenceRepositoryMock
            .Setup(m => m.GetCompanySuggestions(companyId))
            .ReturnsAsync(expectedSuggestions);

        var actualSuggestions = await this.orchestratorUnderTest.GetAllCompanySuggestions(
            companyId,
            suggestion => suggestion.State == CompanySuggestionState.Pending);

        actualSuggestions.Should().NotBeNull();
        actualSuggestions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCompanySuggestion_WithKnownData_ShouldReturnSuccessfully()
    {
        const int companyId = 1901;
        const int suggestedCompanyId = 991102;

        var expectedSuggestion = new CompanySuggestion(suggestedCompanyId);

        var allSuggestions = new List<CompanySuggestion>
        {
            new(11000) { State = CompanySuggestionState.Accepted },
            new(21122) { State = CompanySuggestionState.Accepted },
            expectedSuggestion,
            new(72819) { State = CompanySuggestionState.Declined }
        };

        this.persistenceRepositoryMock
            .Setup(m => m.GetCompanySuggestions(companyId))
            .ReturnsAsync(allSuggestions);

        var actualSuggestion = await this.orchestratorUnderTest.GetCompanySuggestion(companyId, suggestedCompanyId);

        actualSuggestion.Should().NotBeNull();
        actualSuggestion.Should().Be(expectedSuggestion);
    }

    [Fact]
    public async Task GetCompanySuggestion_WithUnknownData_ShouldThrowException()
    {
        const int companyId = 1901;
        const int suggestedCompanyId = 991102;

        var allSuggestions = new List<CompanySuggestion>
        {
            new(11000) { State = CompanySuggestionState.Accepted },
            new(21122) { State = CompanySuggestionState.Accepted },
            new(72819) { State = CompanySuggestionState.Declined }
        };

        this.persistenceRepositoryMock
            .Setup(m => m.GetCompanySuggestions(companyId))
            .ReturnsAsync(allSuggestions);

        await this.orchestratorUnderTest
            .Awaiting(o => o.GetCompanySuggestion(companyId, suggestedCompanyId))
            .Should().ThrowAsync<CompanySuggestionNotFoundException>();
    }

    [Fact]
    public async Task UpdateCompanySuggestion_WithValidData_ShouldPersistUpdate()
    {
        const int companyId = 85139;
        var suggestion = new CompanySuggestion(11071);

        await this.orchestratorUnderTest.UpdateCompanySuggestion(companyId, suggestion);

        this.persistenceRepositoryMock.Verify(m =>
            m.UpdateCompanySuggestion(companyId, suggestion),
            Times.Once);
    }
}
