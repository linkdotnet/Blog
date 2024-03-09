using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Services;

public class UserRecordServiceTests : BunitContext
{
    private readonly IRepository<UserRecord> repositoryMock;
    private readonly BunitNavigationManager fakeNavigationManager;
    private readonly BunitAuthenticationStateProvider fakeAuthenticationStateProvider;
    private readonly UserRecordService sut;

    public UserRecordServiceTests()
    {
        repositoryMock = Substitute.For<IRepository<UserRecord>>();
        fakeNavigationManager = new BunitNavigationManager(this);
        fakeAuthenticationStateProvider = new BunitAuthenticationStateProvider();
        sut = new UserRecordService(
            repositoryMock,
            fakeNavigationManager,
            fakeAuthenticationStateProvider,
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
        recordToDb.DateClicked.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task ShouldNotStoreForAdmin()
    {
        fakeAuthenticationStateProvider.TriggerAuthenticationStateChanged("Steven");

        await sut.StoreUserRecordAsync();

        await repositoryMock.Received(0).StoreAsync(Arg.Any<UserRecord>());
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
