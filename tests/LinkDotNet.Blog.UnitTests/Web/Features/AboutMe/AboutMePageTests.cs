using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.TestUtilities.Fakes;
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

public class AboutMePageTests : BunitContext
{
    public AboutMePageTests()
    {
        ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
    }
    
    [Fact]
    public void ShouldPassIsAuthenticated()
    {
        AddAuthorization().SetAuthorized("test");
        var config = new ProfileInformationBuilder().Build();
        
        var applicationConfiguration = new ApplicationConfigurationBuilder()
            .WithIsAboutMeEnabled(true)
            .Build();
        
        SetupMocks(config, applicationConfiguration);

        var cut = Render<AboutMePage>();

        cut.FindComponent<Profile>().Instance.ShowAdminActions.ShouldBeTrue();
        cut.FindComponent<SkillTable>().Instance.ShowAdminActions.ShouldBeTrue();
        cut.FindComponent<Talks>().Instance.ShowAdminActions.ShouldBeTrue();
    }

    [Fact]
    public void ShouldNotShowWhenEnabledFalse()
    {
        AddAuthorization().SetNotAuthorized();
        var config = new ProfileInformationBuilder().Build();
        
        var applicationConfiguration = new ApplicationConfigurationBuilder()
            .WithIsAboutMeEnabled(false)
            .Build();
        
        SetupMocks(config, applicationConfiguration);

        var cut = Render<AboutMePage>();

        cut.FindComponents<Profile>().Any().ShouldBeFalse();
        cut.FindComponents<SkillTable>().Any().ShouldBeFalse();
    }

    [Fact]
    public void ShouldSetOgData()
    {
        AddAuthorization().SetNotAuthorized();
        var profileInformation = new ProfileInformationBuilder()
            .WithName("My Name")
            .WithProfilePictureUrl("someurl")
            .Build();
        var applicationConfiguration = new ApplicationConfigurationBuilder()
            .WithIsAboutMeEnabled(true)
            .Build();
        SetupMocks(profileInformation, applicationConfiguration);

        var cut = Render<AboutMePage>();

        var ogData = cut.FindComponent<OgData>().Instance;
        ogData.AbsolutePreviewImageUrl.ShouldBe("http://localhost/someurl");
        ogData.Keywords.ShouldNotBeNull();
        ogData.Keywords.ShouldContain("My Name");
        ogData.Title.ShouldContain("About Me - My Name");
        ogData.Description.ShouldNotBeNull();
        ogData.Description.ShouldContain("About Me,My Name");
    }

    [Fact]
    public async Task ShouldRefreshStatsWhenTalksOrSkillsChange()
    {
        AddAuthorization().SetAuthorized("test");
        SetupMocks(new ProfileInformationBuilder().Build(), new ApplicationConfigurationBuilder().WithIsAboutMeEnabled(true).Build());
        var skillRepository = Substitute.For<IRepository<Skill>>();
        skillRepository.GetAllAsync(
                Arg.Any<Expression<Func<Skill, bool>>>(),
                Arg.Any<Expression<Func<Skill, object>>>(),
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Any<int>())
            .Returns(PagedList<Skill>.Empty, new PagedList<Skill>([new SkillBuilder().Build()], 1, 1, 100));
        Services.AddScoped(_ => skillRepository);
        var cut = Render<AboutMePage>();
        cut.FindAll(".stat").ShouldBeEmpty();

        await cut.InvokeAsync(() => cut.FindComponent<TabbedNavigation>().Instance.Changed.InvokeAsync());

        cut.Find(".stat-value").TextContent.ShouldBe("1");
    }

    private void SetupMocks(ProfileInformation config, ApplicationConfiguration applicationConfiguration)
    {
        Services.AddScoped(_ => Options.Create(config));
        Services.AddScoped(_ => Options.Create(applicationConfiguration));
        Services.AddScoped(_ => Substitute.For<ISortOrderCalculator>());
        Services.RegisterRepositoryWithEmptyReturn<ProfileInformationEntry>();
        Services.RegisterRepositoryWithEmptyReturn<Skill>();
        Services.RegisterRepositoryWithEmptyReturn<Talk>();
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddSingleton(TimeProvider.System);
    }
}