using System;
using System.Linq;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.BrokenLinks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BrokenLinks;

public class BrokenLinksPageTests : BunitContext
{
    private readonly IRepository<BrokenLink> repository = Substitute.For<IRepository<BrokenLink>>();

    public BrokenLinksPageTests()
    {
        Services.AddScoped(_ => repository);
    }

    [Fact]
    public void ShouldShowBlogPostAndUrlOfBrokenLinks()
    {
        var brokenLink = BrokenLink.Create("1", "My Post", "https://broken.com/", "404 (NotFound)", new DateTime(2026, 9, 11, 3, 0, 0));
        repository.GetAllAsync(
                Arg.Any<Expression<Func<BrokenLink, bool>>?>(),
                Arg.Any<Expression<Func<BrokenLink, object>>?>(),
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Any<int>())
            .Returns(new PagedList<BrokenLink>([brokenLink], 1, 1, 1));
        RegisterConfiguration(enabled: true);

        var cut = Render<BrokenLinksPage>();

        var cells = cut.FindAll("tbody tr td");
        cells[0].QuerySelector("a")!.GetAttribute("href").ShouldBe("blogPost/1");
        cells[0].TextContent.ShouldBe("My Post");
        cells[1].QuerySelector("a")!.GetAttribute("href").ShouldBe("https://broken.com/");
        cells[1].TextContent.ShouldBe("https://broken.com/");
        cells[2].TextContent.ShouldBe("404 (NotFound)");
    }

    [Fact]
    public void ShouldShowMessageWhenNoBrokenLinks()
    {
        repository.GetAllAsync(
                Arg.Any<Expression<Func<BrokenLink, bool>>?>(),
                Arg.Any<Expression<Func<BrokenLink, object>>?>(),
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Any<int>())
            .Returns(PagedList<BrokenLink>.Empty);
        RegisterConfiguration(enabled: true);

        var cut = Render<BrokenLinksPage>();

        cut.FindAll("#no-broken-links").ShouldHaveSingleItem();
        cut.FindAll("table").ShouldBeEmpty();
    }

    [Fact]
    public void ShouldShowHintAndNotQueryWhenCheckerIsDisabled()
    {
        RegisterConfiguration(enabled: false);

        var cut = Render<BrokenLinksPage>();

        cut.FindAll("#broken-link-checker-disabled").ShouldHaveSingleItem();
        repository.ReceivedCalls().ShouldBeEmpty();
    }

    private void RegisterConfiguration(bool enabled)
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().WithEnableBrokenLinkChecker(enabled).Build()));
    }
}
