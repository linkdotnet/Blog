using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Archive;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;
using X.PagedList;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Archive;

public class ArchiveTests : TestContext
{
    [Fact]
    public void ShouldDisplayAllBlogPosts()
    {
        var repository = new Mock<IRepository<BlogPost>>();
        Services.AddScoped(_ => repository.Object);
        var allBlogPosts = new[]
        {
            CreateBlogPost(new DateTime(2021, 1, 1), "Blog Post 1"),
            CreateBlogPost(new DateTime(2021, 2, 1), "Blog Post 2"),
            CreateBlogPost(new DateTime(2022, 1, 1), "Blog Post 3"),
        }.OrderByDescending(a => a.UpdatedDate);
        repository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<Expression<Func<BlogPost, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<BlogPost>(allBlogPosts, 1, 10));

        var cut = RenderComponent<ArchivePage>();

        cut.WaitForElements("h2");
        var yearHeader = cut.FindAll("h2");
        yearHeader.Should().HaveCount(2);
        yearHeader[0].TextContent.Should().Be("2022");
        yearHeader[1].TextContent.Should().Be("2021");
        var entries = cut.FindAll("li");
        entries.Should().HaveCount(3);
        entries[0].TextContent.Should().Be("Blog Post 3");
        entries[1].TextContent.Should().Be("Blog Post 2");
        entries[2].TextContent.Should().Be("Blog Post 1");
    }

    [Fact]
    public void ShouldShowLoading()
    {
        var repository = new Mock<IRepository<BlogPost>>();
        Services.AddScoped(_ => repository.Object);
        repository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<Expression<Func<BlogPost, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
        .Returns(async () =>
        {
            await Task.Delay(250);
            return new PagedList<BlogPost>(Array.Empty<BlogPost>(), 1, 1);
        });

        var cut = RenderComponent<ArchivePage>();

        cut.FindComponents<Loading>().Count.Should().Be(1);
    }

    private static BlogPost CreateBlogPost(DateTime date, string title)
    {
        return new BlogPostBuilder()
            .WithTitle(title)
            .WithUpdatedDate(date)
            .Build();
    }
}