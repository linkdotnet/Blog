using System.Linq;
using AngleSharp.Html.Dom;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class BlogPostNavigationTests : BunitContext
{
    [Fact]
    public void ShouldFireEventWhenGoingToNextPage()
    {
        var page = CreatePagedList(2, 3);

        var cut = Render<BlogPostNavigation<BlogPost>>(p =>
            p.Add(param => param.PageList, page));

        cut.FindAll("a").Cast<IHtmlAnchorElement>().Last().Href.ShouldEndWith("/3");
    }

    [Fact]
    public void ShouldFireEventWhenGoingToPreviousPage()
    {
        var page = CreatePagedList(2, 3);

        var cut = Render<BlogPostNavigation<BlogPost>>(p =>
            p.Add(param => param.PageList, page));

        cut.FindAll("a").Cast<IHtmlAnchorElement>().First().Href.ShouldEndWith("/1");
    }

    [Fact]
    public void ShouldNotFireNextWhenOnLastPage()
    {
        var page = CreatePagedList(2, 2);
        var cut = Render<BlogPostNavigation<BlogPost>>(p =>
            p.Add(param => param.PageList, page));

        cut.Find("li:last-child").ClassList.ShouldContain("disabled");
    }

    [Fact]
    public void ShouldNotFireNextWhenOnFirstPage()
    {
        var page = CreatePagedList(1, 2);
        var cut = Render<BlogPostNavigation<BlogPost>>(p =>
            p.Add(param => param.PageList, page));

        cut.Find("li:first-child").ClassList.ShouldContain("disabled");
    }

    [Fact]
    public void ShouldNotFireNextWhenNoPage()
    {
        var page = CreatePagedList(0, 0);
        var cut = Render<BlogPostNavigation<BlogPost>>(p =>
            p.Add(param => param.PageList, page));

        cut.Find("li:first-child").ClassList.ShouldContain("disabled");
        cut.Find("li:last-child").ClassList.ShouldContain("disabled");
    }

    private static IPagedList<BlogPost> CreatePagedList(int currentPage, int pageCount)
    {
        var page = Substitute.For<IPagedList<BlogPost>>();
        page.PageNumber.Returns(currentPage);
        page.IsFirstPage.Returns(currentPage == 1);
        page.IsLastPage.Returns(currentPage == pageCount);

        return page;
    }
}
