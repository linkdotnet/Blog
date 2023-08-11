using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Services;

public class UserRecordServiceTests : TestContext
{
    private readonly IRepository<UserRecord> repositoryMock;
    private readonly FakeNavigationManager fakeNavigationManager;
    private readonly FakeAuthenticationStateProvider fakeAuthenticationStateProvider;
    private readonly ILocalStorageService localStorageService;
    private readonly UserRecordService sut;

    public UserRecordServiceTests()
    {
        repositoryMock = Substitute.For<IRepository<UserRecord>>();
        fakeNavigationManager = new FakeNavigationManager(Renderer);
        fakeAuthenticationStateProvider = new FakeAuthenticationStateProvider();
        localStorageService = Substitute.For<ILocalStorageService>();
        sut = new UserRecordService(
            repositoryMock,
            fakeNavigationManager,
            fakeAuthenticationStateProvider,
            localStorageService,
            Substitute.For<ILogger<UserRecordService>>());
    }

    [Fact]
    public async Task ShouldStoreInformation()
    {
        const string url = "http://localhost/subpart";
        fakeNavigationManager.NavigateTo(url);
        UserRecord recordToDb = null;
        repositoryMock.When(r => r.StoreAsync(Arg.Any<UserRecord>()))
            .Do(call => recordToDb = call.Arg<UserRecord>());

        await sut.StoreUserRecordAsync();

        recordToDb.Should().NotBeNull();
        recordToDb.UrlClicked.Should().Be("subpart");
        recordToDb.UserIdentifierHash.Should().NotBe(0);
        recordToDb.DateClicked.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task ShouldGetUserHashAgain()
    {
        UserRecord recordToDb = null;
        var guidForUser = Guid.NewGuid();
        localStorageService.ContainKeyAsync("user").Returns(true);
        localStorageService.GetItemAsync<Guid>("user").Returns(guidForUser);
        repositoryMock.When(r => r.StoreAsync(Arg.Any<UserRecord>()))
            .Do(call => recordToDb = call.Arg<UserRecord>());

        await sut.StoreUserRecordAsync();

        recordToDb.Should().NotBeNull();
        var hashCode = guidForUser.GetHashCode();
        recordToDb.UserIdentifierHash.Should().Be(hashCode);
    }

    [Fact]
    public async Task ShouldNotStoreForAdmin()
    {
        fakeAuthenticationStateProvider.TriggerAuthenticationStateChanged("Steven");

        await sut.StoreUserRecordAsync();

        await repositoryMock.Received(0).StoreAsync(Arg.Any<UserRecord>());
    }

    [Fact]
    public void ShouldNotThrowExceptionToOutsideWorld()
    {
        localStorageService.When(l => l.SetItemAsync("user", Arg.Any<Guid>()))
            .Do(_ => throw new Exception());

        var act = () => sut.StoreUserRecordAsync();

        act.Should().NotThrow<Exception>();
    }

    [Fact]
    public async Task ShouldReturnFalseWhenContainKeyOnExceptionAndCreateNewOne()
    {
        localStorageService.When(l => l.ContainKeyAsync("user"))
            .Do(_ => throw new Exception());

        await sut.StoreUserRecordAsync();

        await repositoryMock.Received(1).StoreAsync(Arg.Any<UserRecord>());
        await localStorageService.Received(1).SetItemAsync("user", Arg.Any<Guid>());

    }

    [InlineData("http://localhost/blogPost/12?q=3", "blogPost/12")]
    [InlineData("http://localhost/?q=3", "")]
    [InlineData("", "")]
    [InlineData("http://localhost/someroute/subroute", "someroute/subroute")]
    [Theory]
    public async Task ShouldRemoveQueryStringIfPresent(string url, string expectedRecord)
    {
        fakeNavigationManager.NavigateTo(url);
        UserRecord recordToDb = null;
        repositoryMock.When(r => r.StoreAsync(Arg.Any<UserRecord>()))
            .Do(call => recordToDb = call.Arg<UserRecord>());

        await sut.StoreUserRecordAsync();

        recordToDb.Should().NotBeNull();
        recordToDb.UrlClicked.Should().Be(expectedRecord);
    }
}
