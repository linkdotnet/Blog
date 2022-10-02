using System.Linq;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.AboutMe;
using LinkDotNet.Blog.Web.Features.AboutMe.Components;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Skill;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Talk;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe;

public class AboutMePageTests : TestContext
{
    [Fact]
    public void ShouldPassIsAuthenticated()
    {
        this.AddTestAuthorization().SetAuthorized("test");
        var config = CreateAppConfiguration(new ProfileInformation { ProfilePictureUrl = string.Empty });
        SetupMocks(config);

        var cut = RenderComponent<AboutMePage>();

        cut.FindComponent<Profile>().Instance.ShowAdminActions.Should().BeTrue();
        cut.FindComponent<SkillTable>().Instance.ShowAdminActions.Should().BeTrue();
        cut.FindComponent<Talks>().Instance.ShowAdminActions.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotShowWhenEnabledFalse()
    {
        this.AddTestAuthorization().SetNotAuthorized();
        var config = CreateAppConfiguration();
        SetupMocks(config);

        var cut = RenderComponent<AboutMePage>();

        cut.FindComponents<Profile>().Any().Should().BeFalse();
        cut.FindComponents<SkillTable>().Any().Should().BeFalse();
    }

    [Fact]
    public void ShouldSetOgData()
    {
        this.AddTestAuthorization().SetNotAuthorized();
        var profileInformation = new ProfileInformation
        {
            Name = "My Name",
            ProfilePictureUrl = "someurl",
        };
        var config = CreateAppConfiguration(profileInformation);
        SetupMocks(config);

        var cut = RenderComponent<AboutMePage>();

        var ogData = cut.FindComponent<OgData>().Instance;
        ogData.AbsolutePreviewImageUrl.Should().Be("http://localhost/someurl");
        ogData.Keywords.Should().Contain("My Name");
        ogData.Title.Should().Contain("About Me - My Name");
        ogData.Description.Should().Contain("About Me,My Name");
    }

    private static AppConfiguration CreateAppConfiguration(ProfileInformation info = null)
    {
        return new AppConfiguration
        {
            ProfileInformation = info,
        };
    }

    private void SetupMocks(AppConfiguration config)
    {
        Services.AddScoped(_ => config);
        Services.AddScoped(_ => Mock.Of<IUserRecordService>());
        Services.AddScoped(_ => Mock.Of<ISortOrderCalculator>());
        Services.RegisterRepositoryWithEmptyReturn<ProfileInformationEntry>();
        Services.RegisterRepositoryWithEmptyReturn<Skill>();
        Services.RegisterRepositoryWithEmptyReturn<Talk>();
        Services.AddScoped(_ => Mock.Of<IToastService>());
    }
}