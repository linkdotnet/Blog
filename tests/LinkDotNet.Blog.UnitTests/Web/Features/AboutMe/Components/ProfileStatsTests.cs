using System;
using System.Linq;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.AboutMe.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe.Components;

public class ProfileStatsTests : BunitContext
{
    public ProfileStatsTests()
    {
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2026, 11, 9, 12, 0, 0, TimeSpan.Zero));
        timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        Services.AddSingleton(timeProvider);
    }

    [Fact]
    public void ShouldShowCountsAndNextTalk()
    {
        RegisterTalks(
            new TalkBuilder().WithPublishedDate(new DateTime(2021, 7, 6)).Build(),
            new TalkBuilder().WithPublishedDate(new DateTime(2025, 5, 22)).Build(),
            new TalkBuilder().WithPublishedDate(new DateTime(2026, 11, 19)).Build());
        RegisterSkills(
            new SkillBuilder().WithCapability("Backend").Build(),
            new SkillBuilder().WithCapability("backend").Build(),
            new SkillBuilder().WithCapability("Frontend").Build());

        var cut = Render<ProfileStats>();

        var stats = cut.FindAll(".stat").Select(s => s.TextContent.Split((char[])['\n', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).ToList();
        stats[0].ShouldBe(["Talks", "3", "since 2021"]);
        stats[1].ShouldBe(["Skills", "3", "in 2 areas"]);
        stats[2].ShouldBe(["Next talk", "19 Nov", "in 10 days"]);
    }

    [Fact]
    public void ShouldShowLatestTalkWhenNothingIsUpcoming()
    {
        RegisterTalks(
            new TalkBuilder().WithPublishedDate(new DateTime(2024, 4, 18)).Build(),
            new TalkBuilder().WithPublishedDate(new DateTime(2025, 5, 22)).Build());
        RegisterSkills();

        var cut = Render<ProfileStats>();

        var spotlight = cut.FindAll(".stat")[^1];
        spotlight.QuerySelector("dt")!.TextContent.ShouldBe("Latest talk");
        spotlight.QuerySelector(".stat-value")!.TextContent.Trim().ShouldBe("22 May");
        spotlight.QuerySelector(".stat-detail")!.TextContent.ShouldBe("2025");
    }

    [Fact]
    public void ShouldRenderNothingWithoutTalksAndSkills()
    {
        RegisterTalks();
        RegisterSkills();

        var cut = Render<ProfileStats>();

        cut.Markup.Trim().ShouldBeEmpty();
    }

    private void RegisterTalks(params Talk[] talks) => Register(talks);

    private void RegisterSkills(params Skill[] skills) => Register(skills);

    private void Register<TEntity>(TEntity[] entities)
        where TEntity : Entity
    {
        var repository = Substitute.For<IRepository<TEntity>>();
        repository.GetAllAsync(
                Arg.Any<Expression<Func<TEntity, bool>>>(),
                Arg.Any<Expression<Func<TEntity, object>>>(),
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Any<int>())
            .Returns(new PagedList<TEntity>(entities, entities.Length, 1, 100));
        Services.AddScoped(_ => repository);
    }
}
