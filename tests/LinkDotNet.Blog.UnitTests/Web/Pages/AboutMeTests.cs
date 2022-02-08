using System;
using System.Linq;
using System.Linq.Expressions;
using Blazored.Toast.Services;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;
using X.PagedList;

namespace LinkDotNet.Blog.UnitTests.Web.Pages;

public class AboutMeTests : TestContext
{
    [Fact]
    public void ShouldPassIsAuthenticated()
    {
        this.AddTestAuthorization().SetAuthorized("test");
        var config = CreateAppConfiguration(new ProfileInformation { ProfilePictureUrl = string.Empty });
        SetupMocks(config);

        var cut = RenderComponent<AboutMe>();

        cut.FindComponent<Profile>().Instance.IsAuthenticated.Should().BeTrue();
        cut.FindComponent<SkillTable>().Instance.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void ShouldNotShowWhenEnabledFalse()
    {
        this.AddTestAuthorization().SetNotAuthorized();
        var config = CreateAppConfiguration();
        SetupMocks(config);

        var cut = RenderComponent<AboutMe>();

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

        var cut = RenderComponent<AboutMe>();

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
        var skillRepo = new Mock<IRepository<Skill>>();
        skillRepo.Setup(s => s.GetAllAsync(
                It.IsAny<Expression<Func<Skill, bool>>>(),
                It.IsAny<Expression<Func<Skill, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<Skill>(Array.Empty<Skill>(), 1, 1));
        var profileRepo = new Mock<IRepository<ProfileInformationEntry>>();
        profileRepo.Setup(s => s.GetAllAsync(
                It.IsAny<Expression<Func<ProfileInformationEntry, bool>>>(),
                It.IsAny<Expression<Func<ProfileInformationEntry, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<ProfileInformationEntry>(Array.Empty<ProfileInformationEntry>(), 1, 1));

        Services.AddScoped(_ => config);
        Services.AddScoped(_ => Mock.Of<IUserRecordService>());
        Services.AddScoped(_ => Mock.Of<ISortOrderCalculator>());
        Services.AddScoped(_ => skillRepo.Object);
        Services.AddScoped(_ => profileRepo.Object);
        Services.AddScoped(_ => Mock.Of<IToastService>());
    }
}