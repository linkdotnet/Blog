using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Talk;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.AboutMe.Components;

public sealed class TalksTests : SqlDatabaseTestBase<Talk>, IDisposable
{
    private readonly TestContext ctx = new();

    public TalksTests()
    {
        ctx.Services.AddScoped(_ => Repository);
    }

    [Fact]
    public void WhenAddingTalkThenDisplayedToUser()
    {
        var cut = ctx.RenderComponent<Talks>(
            p => p.Add(s => s.ShowAdminActions, true));
        cut.Find("#add-talk").Click();
        cut.Find("#talk-title").Change("title");
        cut.Find("#talk-place").Change("Zurich");
        cut.Find("#talk-date").Change(new DateTime(2022, 10, 2));
        cut.Find("#talk-content").Change("text");

        cut.Find("#talk-submit").Click();

        cut.WaitForState(() => cut.HasComponent<TalkEntry>());
        var entry = cut.FindComponent<TalkEntry>();
        entry.Find("#talk-display-content strong").TextContent.Should().Be("title");
        entry.Find("#talk-place").TextContent.Should().Be("Zurich");
        entry.Find("#talk-description p").TextContent.Should().Be("text");
    }

    [Fact]
    public async Task LoadEarlierSavedTalks()
    {
        await Repository.StoreAsync(new TalkBuilder().Build());
        await Repository.StoreAsync(new TalkBuilder().Build());

        var cut = ctx.RenderComponent<Talks>();

        cut.WaitForState(() => cut.HasComponent<TalkEntry>());
        cut.FindComponents<TalkEntry>().Count.Should().Be(2);
    }

    [Fact]
    public async Task WhenUserClickDeleteButtonThenDeleted()
    {
        await Repository.StoreAsync(new TalkBuilder().Build());
        var cut = ctx.RenderComponent<Talks>(
            p => p.Add(s => s.ShowAdminActions, true));
        cut.WaitForState(() => cut.HasComponent<TalkEntry>());

        cut.FindComponent<TalkEntry>().Find("#talk-delete").Click();

        cut.WaitForState(() => !cut.HasComponent<TalkEntry>());
        cut.HasComponent<TalkEntry>().Should().BeFalse();
        (await DbContext.Talks.AnyAsync()).Should().BeFalse();
    }

    [Fact]
    public async Task TalksAreOrderFromNewestToLatest()
    {
        await Repository.StoreAsync(new TalkBuilder().WithPublishedDate(new DateTime(2021, 1, 1)).Build());
        await Repository.StoreAsync(new TalkBuilder().WithPublishedDate(new DateTime(2022, 1, 1)).Build());

        var cut = ctx.RenderComponent<Talks>(
            p => p.Add(s => s.ShowAdminActions, true));

        cut.WaitForState(() => cut.HasComponent<TalkEntry>());
        var talks = cut.FindComponents<TalkEntry>();
        talks.Count.Should().Be(2);
        talks[0].Instance.Talk.PublishedDate.Should().Be(new DateTime(2022, 1, 1));
        talks[1].Instance.Talk.PublishedDate.Should().Be(new DateTime(2021, 1, 1));
    }

    public void Dispose()
    {
        ctx?.Dispose();
    }
}