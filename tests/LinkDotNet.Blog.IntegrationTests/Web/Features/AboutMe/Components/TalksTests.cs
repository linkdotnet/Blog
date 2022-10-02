using System;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Talk;
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
    public void WhenAddingATalkThenDisplayedToUser()
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
        entry.Find("#talk-description").TextContent.Should().Be("text");
    }

    public void Dispose()
    {
        ctx?.Dispose();
    }
}