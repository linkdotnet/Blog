using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Domain;
using Moq;
using X.PagedList;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
    public class BlogPostNavigationTests : TestContext
    {
        [Fact]
        public void ShouldFireEventWhenGoingToNextPage()
        {
            var actualNewPage = 0;
            var page = CreatePagedList(2, 3);
            var cut = RenderComponent<BlogPostNavigation>(p => p.Add(param => param.CurrentPage, page.Object)
                .Add(param => param.OnPageChanged, newPage => actualNewPage = newPage));

            cut.Find("li:last-child a").Click();

            actualNewPage.Should().Be(3);
        }

        [Fact]
        public void ShouldFireEventWhenGoingToPreviousPage()
        {
            var actualNewPage = 0;
            var page = CreatePagedList(2, 3);
            var cut = RenderComponent<BlogPostNavigation>(p => p.Add(param => param.CurrentPage, page.Object)
                .Add(param => param.OnPageChanged, newPage => actualNewPage = newPage));

            cut.Find("li:first-child a").Click();

            actualNewPage.Should().Be(1);
        }

        [Fact]
        public void ShouldNotFireNextWhenOnLastPage()
        {
            var page = CreatePagedList(2, 2);
            var cut = RenderComponent<BlogPostNavigation>(p =>
                p.Add(param => param.CurrentPage, page.Object));

            cut.Find("li:last-child").ClassList.Should().Contain("disabled");
        }

        [Fact]
        public void ShouldNotFireNextWhenOnFirstPage()
        {
            var page = CreatePagedList(1, 2);
            var cut = RenderComponent<BlogPostNavigation>(p =>
                p.Add(param => param.CurrentPage, page.Object));

            cut.Find("li:first-child").ClassList.Should().Contain("disabled");
        }

        [Fact]
        public void ShouldNotFireNextWhenNoPage()
        {
            var page = CreatePagedList(0, 0);
            var cut = RenderComponent<BlogPostNavigation>(p =>
                p.Add(param => param.CurrentPage, page.Object));

            cut.Find("li:first-child").ClassList.Should().Contain("disabled");
            cut.Find("li:last-child").ClassList.Should().Contain("disabled");
        }

        private static Mock<IPagedList<BlogPost>> CreatePagedList(int currentPage, int pageCount)
        {
            var page = new Mock<IPagedList<BlogPost>>();
            page.Setup(p => p.PageNumber).Returns(currentPage);
            page.Setup(p => p.PageCount).Returns(pageCount);
            page.Setup(p => p.IsFirstPage).Returns(currentPage == 1);
            page.Setup(p => p.IsLastPage).Returns(currentPage == pageCount);

            return page;
        }
    }
}