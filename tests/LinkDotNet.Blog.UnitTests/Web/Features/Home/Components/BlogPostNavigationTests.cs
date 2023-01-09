using System.Linq;
using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class BlogPostNavigationTests : TestContext
{
    [Fact]
    public void ShouldFireEventWhenGoingToNextPage()
    {
        var page = CreatePagedList(2, 3);

        var cut = RenderComponent<BlogPostNavigation>(p =>
            p.Add(param => param.PageList, page.Object));

        cut.FindAll("a").Cast<IHtmlAnchorElement>().Last().Href.Should().EndWith("/3");
    }

    [Fact]
    public void ShouldFireEventWhenGoingToPreviousPage()
    {
        var page = CreatePagedList(2, 3);

        var cut = RenderComponent<BlogPostNavigation>(p =>
            p.Add(param => param.PageList, page.Object));

        cut.FindAll("a").Cast<IHtmlAnchorElement>().First().Href.Should().EndWith("/1");
    }

    [Fact]
    public void ShouldNotFireNextWhenOnLastPage()
    {
        var page = CreatePagedList(2, 2);
        var cut = RenderComponent<BlogPostNavigation>(p =>
            p.Add(param => param.PageList, page.Object));

        cut.Find("li:last-child").ClassList.Should().Contain("disabled");
    }

    [Fact]
    public void ShouldNotFireNextWhenOnFirstPage()
    {
        var page = CreatePagedList(1, 2);
        var cut = RenderComponent<BlogPostNavigation>(p =>
            p.Add(param => param.PageList, page.Object));

        cut.Find("li:first-child").ClassList.Should().Contain("disabled");
    }

    [Fact]
    public void ShouldNotFireNextWhenNoPage()
    {
        var page = CreatePagedList(0, 0);
        var cut = RenderComponent<BlogPostNavigation>(p =>
            p.Add(param => param.PageList, page.Object));

        cut.Find("li:first-child").ClassList.Should().Contain("disabled");
        cut.Find("li:last-child").ClassList.Should().Contain("disabled");
    }

    private static Mock<IPagedList<BlogPost>> CreatePagedList(int currentPage, int pageCount)
    {
        var page = new Mock<IPagedList<BlogPost>>();
        page.Setup(p => p.PageNumber).Returns(currentPage);
        page.Setup(p => p.IsFirstPage).Returns(currentPage == 1);
        page.Setup(p => p.IsLastPage).Returns(currentPage == pageCount);

        return page;
    }
}
