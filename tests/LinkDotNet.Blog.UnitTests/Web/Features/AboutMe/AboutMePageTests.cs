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
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe;

public class AboutMePageTests : TestContext
{
    [Fact]
    public void ShouldPassIsAuthenticated()
    {
        this.AddTestAuthorization().SetAuthorized("test");
        var config = new ProfileInformation { ProfilePictureUrl = string.Empty };
        
        var applicationConfiguration = new ApplicationConfiguration
        {
            IsAboutMeEnabled = true
        };
        
        SetupMocks(config, applicationConfiguration);

        var cut = RenderComponent<AboutMePage>();

        cut.FindComponent<Profile>().Instance.ShowAdminActions.Should().BeTrue();
        cut.FindComponent<SkillTable>().Instance.ShowAdminActions.Should().BeTrue();
        cut.FindComponent<Talks>().Instance.ShowAdminActions.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotShowWhenEnabledFalse()
    {
        this.AddTestAuthorization().SetNotAuthorized();
        var config = new ProfileInformation();
        
        var applicationConfiguration = new ApplicationConfiguration
        {
            IsAboutMeEnabled = false
        };
        
        SetupMocks(config, applicationConfiguration);

        var cut = RenderComponent<AboutMePage>();

        cut.FindComponents<Profile>().Any().Should().BeFalse();
        cut.FindComponents<SkillTable>().Any().Should().BeFalse();
    }

    [Fact]
    public void ShouldSetOgData()
    {
        ComponentFactories.AddStub<Profile>();
        this.AddTestAuthorization().SetNotAuthorized();
        var profileInformation = new ProfileInformation
        {
            Name = "My Name",
            ProfilePictureUrl = "someurl",
        };
        var applicationConfiguration = new ApplicationConfiguration
        {
            IsAboutMeEnabled = true
        };
        SetupMocks(profileInformation, applicationConfiguration);

        var cut = RenderComponent<AboutMePage>();

        var ogData = cut.FindComponent<OgData>().Instance;
        ogData.AbsolutePreviewImageUrl.Should().Be("http://localhost/someurl");
        ogData.Keywords.Should().Contain("My Name");
        ogData.Title.Should().Contain("About Me - My Name");
        ogData.Description.Should().Contain("About Me,My Name");
    }

    private void SetupMocks(ProfileInformation config, ApplicationConfiguration applicationConfiguration)
    {
        Services.AddScoped(_ => Options.Create(config));
        Services.AddScoped(_ => Options.Create(applicationConfiguration));
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        Services.AddScoped(_ => Substitute.For<ISortOrderCalculator>());
        Services.RegisterRepositoryWithEmptyReturn<ProfileInformationEntry>();
        Services.RegisterRepositoryWithEmptyReturn<Skill>();
        Services.RegisterRepositoryWithEmptyReturn<Talk>();
        Services.AddScoped(_ => Substitute.For<IToastService>());
    }
}