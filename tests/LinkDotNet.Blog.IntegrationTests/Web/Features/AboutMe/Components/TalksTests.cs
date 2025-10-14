using System;
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

    public TalksTests()
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
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
        entry.Find("#talk-display-content strong").TextContent.ShouldBe("title");
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

    public void Dispose()
    {
        ctx?.Dispose();
    }
}
