using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Services;

public class CurrentUserServiceTests : BunitContext
{
    private readonly BunitAuthenticationStateProvider fakeAuthenticationStateProvider;
    private readonly CurrentUserService sut;

    public CurrentUserServiceTests()
    {
        fakeAuthenticationStateProvider = new BunitAuthenticationStateProvider();
        sut = new CurrentUserService(fakeAuthenticationStateProvider);
    }

    [Theory]
    [InlineData("name")]
    [InlineData("preferred_username")]
    [InlineData("nickname")]
    public async Task ShouldGetDisplayNameWhenAuthenticated(string claimType)
    {
        var claims = new List<Claim>()
        {
            new Claim(claimType, "Test Author")
        };

        fakeAuthenticationStateProvider.TriggerAuthenticationStateChanged("Steven", claims: claims);
        var authorName = await sut.GetDisplayNameAsync();
        authorName.ShouldBe("Test Author");
    }

    [Fact]
    public async Task ShouldGetNullAsDisplayNameWhenNoClaimGiven()
    {
        fakeAuthenticationStateProvider.TriggerAuthenticationStateChanged("Steven");
        var authorName = await sut.GetDisplayNameAsync();
        authorName.ShouldBeNull();
    }

    [Fact]
    public async Task ShouldGetNullAsDisplayNameWhenUnauthenticated()
    {
        var authorName = await sut.GetDisplayNameAsync();
        authorName.ShouldBeNull();
    }
}
