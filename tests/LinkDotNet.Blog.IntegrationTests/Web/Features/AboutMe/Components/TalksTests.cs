using System;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.TestUtilities.Fakes;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Talk;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.AboutMe.Components;

public sealed class TalksTests : SqlDatabaseTestBase<Talk>, IDisposable
{
    private readonly BunitContext ctx = new();
    private readonly TimeProvider timeProvider = Substitute.For<TimeProvider>();

    public TalksTests()
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddSingleton(timeProvider);
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2026, 11, 9, 12, 0, 0, TimeSpan.Zero));
        timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        ctx.JSInterop.SetupNativeDialog();
    }

    [Fact]
    public void WhenAddingTalkThenDisplayedToUser()
    {
        var cut = ctx.Render<Talks>(
            p => p.Add(s => s.ShowAdminActions, true));
        cut.Find("#add-talk").Click();
        cut.Find("#talk-title").Change("title");
        cut.Find("#talk-place").Change("Zurich");
        cut.Find("#talk-date").Change(new DateTime(2022, 10, 2));
        cut.Find("#talk-content").Input("text");

        cut.Find("form").Submit();

        var entry = cut.WaitForComponent<TalkEntry>();
        entry.Find("#talk-display-content .talk-title").TextContent.ShouldBe("title");
        entry.Find("#talk-place").TextContent.ShouldBe("Zurich");
        entry.Find("#talk-description p").TextContent.ShouldBe("text");
    }

    [Fact]
    public async Task LoadEarlierSavedTalks()
    {
        await Repository.StoreAsync(new TalkBuilder().Build());
        await Repository.StoreAsync(new TalkBuilder().Build());

        var cut = ctx.Render<Talks>();

        cut.WaitForComponents<TalkEntry>().Count.ShouldBe(2);
    }

    [Fact]
    public async Task WhenUserClickDeleteButtonThenDeleted()
    {
        await Repository.StoreAsync(new TalkBuilder().Build());
        var cut = ctx.Render<Talks>(
            p => p.Add(s => s.ShowAdminActions, true));

        await cut.WaitForComponent<TalkEntry>().Find("#talk-delete").ClickAsync();

        cut.WaitForState(() => !cut.HasComponent<TalkEntry>());
        cut.HasComponent<TalkEntry>().ShouldBeFalse();
        (await DbContext.Talks.AnyAsync(cancellationToken: TestContext.Current.CancellationToken)).ShouldBeFalse();
    }

    [Fact]
    public async Task TalksAreOrderFromNewestToLatest()
    {
        await Repository.StoreAsync(new TalkBuilder().WithPublishedDate(new DateTime(2021, 1, 1)).Build());
        await Repository.StoreAsync(new TalkBuilder().WithPublishedDate(new DateTime(2022, 1, 1)).Build());

        var cut = ctx.Render<Talks>(
            p => p.Add(s => s.ShowAdminActions, true));

        var talks = cut.WaitForComponents<TalkEntry>();
        talks.Count.ShouldBe(2);
        talks[0].Instance.Talk.PublishedDate.ShouldBe(new DateTime(2022, 1, 1));
        talks[1].Instance.Talk.PublishedDate.ShouldBe(new DateTime(2021, 1, 1));
    }

    [Fact]
    public async Task ShouldNotifyChangeAfterDelete()
    {
        await Repository.StoreAsync(new TalkBuilder().Build());
        var changed = false;
        var cut = ctx.Render<Talks>(p => p
            .Add(s => s.ShowAdminActions, true)
            .Add(s => s.Changed, () => changed = true));

        await cut.WaitForComponent<TalkEntry>().Find("#talk-delete").ClickAsync();

        changed.ShouldBeTrue();
    }

    [Fact]
    public async Task UpcomingTalksAreShownFirstWithBadge()
    {
        await Repository.StoreAsync(new TalkBuilder().WithTitle("Past").WithPublishedDate(new DateTime(2025, 5, 22)).Build());
        await Repository.StoreAsync(new TalkBuilder().WithTitle("Later").WithPublishedDate(new DateTime(2027, 3, 1)).Build());
        await Repository.StoreAsync(new TalkBuilder().WithTitle("Soon").WithPublishedDate(new DateTime(2027, 1, 28)).Build());

        var cut = ctx.Render<Talks>();

        var talks = cut.WaitForComponents<TalkEntry>();
        talks.Select(t => t.Instance.Talk.PresentationTitle).ShouldBe(["Soon", "Later", "Past"]);
        talks.Select(t => t.Instance.IsUpcoming).ShouldBe([true, true, false]);
        cut.FindAll(".talk-group-title span:first-child").Select(e => e.TextContent).ShouldBe(["Upcoming", "2025"]);
        cut.FindAll(".talk-badge").Count.ShouldBe(2);
    }

    public void Dispose()
    {
        ctx?.Dispose();
    }
}
