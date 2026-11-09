using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Bookmarks.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Bookmarks;

public class BookmarksTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldOnlyDisplayBookmarkedPosts()
    {
        // Arrange
        using var ctx = new BunitContext();
        var bookmarkService = Substitute.For<IBookmarkService>();
        var bookmarkedBlogPost = new BlogPostBuilder().WithTitle("Bookmarked Post").Build();
        var nonBookmarkedBlogPost = new BlogPostBuilder().WithTitle("Non-Bookmarked Post").Build();
        await Repository.StoreAsync(bookmarkedBlogPost);
        await Repository.StoreAsync(nonBookmarkedBlogPost);
        bookmarkService.GetBookmarkedPostIds().Returns(new List<string> { bookmarkedBlogPost.Id });
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => bookmarkService);

        // Act
        var cut = ctx.Render<Blog.Web.Features.Bookmarks.Bookmarks>();
        
        // Assert
        cut.WaitForElement(".bookmark-row");
        var rows = cut.FindComponents<BookmarkRow>();

        rows.ShouldHaveSingleItem();
        rows[0].Find(".row-title").TextContent.ShouldBe("Bookmarked Post");
    }
    
    [Fact]
    public async Task ShouldRemoveBookmarkWhenButtonIsClicked()
    {
        // Arrange
        using var ctx = new BunitContext();
        var bookmarkService = Substitute.For<IBookmarkService>();
        var bookmarkedBlogPost = new BlogPostBuilder().WithTitle("Bookmarked Post").Build();
        await Repository.StoreAsync(bookmarkedBlogPost);
        bookmarkService.GetBookmarkedPostIds().Returns(new List<string> { bookmarkedBlogPost.Id });
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => bookmarkService);
        
        // Act
        var cut = ctx.Render<Blog.Web.Features.Bookmarks.Bookmarks>();
        cut.WaitForElement(".bookmark-row");

        await cut.Find(".remove-button").ClickAsync();

        // Assert
        await bookmarkService.Received(1).SetBookmark(bookmarkedBlogPost.Id, false);
        cut.FindAll(".bookmark-row").ShouldBeEmpty();
        cut.Find(".undo-message").TextContent.ShouldContain("Bookmarked Post");
    }
    
    [Fact]
    public void ShouldDisplayMessageWhenNoBookmarksExist()
    {
        // Arrange
        using var ctx = new BunitContext();
        var bookmarkService = Substitute.For<IBookmarkService>();
        bookmarkService.GetBookmarkedPostIds().Returns(new List<string>());
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => bookmarkService);
        
        // Act
        var cut = ctx.Render<Blog.Web.Features.Bookmarks.Bookmarks>();
        
        // Assert
        cut.WaitForElement(".empty-title");
        cut.Find(".empty-title").TextContent.ShouldBe("No Bookmarks Yet");
        cut.FindComponents<BookmarkRow>().Count.ShouldBe(0);
    }

    [Fact]
    public async Task ShouldRestoreBookmarkWhenUndoIsClicked()
    {
        // Arrange
        using var ctx = new BunitContext();
        var bookmarkService = Substitute.For<IBookmarkService>();
        var blogPost = new BlogPostBuilder().WithTitle("Bookmarked Post").Build();
        await Repository.StoreAsync(blogPost);
        bookmarkService.GetBookmarkedPostIds().Returns(new List<string> { blogPost.Id });
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => bookmarkService);
        var cut = ctx.Render<Blog.Web.Features.Bookmarks.Bookmarks>();
        cut.WaitForElement(".bookmark-row");
        await cut.Find(".remove-button").ClickAsync();

        // Act
        await cut.Find(".undo-button").ClickAsync();

        // Assert
        await bookmarkService.Received(1).SetBookmark(blogPost.Id, true);
        cut.FindAll(".bookmark-row").Count.ShouldBe(1);
        cut.FindAll(".undo-bar").ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldRestoreRemovedPostWithoutDismissingNewerUndo()
    {
        // Arrange
        using var ctx = new BunitContext();
        var bookmarkService = Substitute.For<IBookmarkService>();
        var firstPost = new BlogPostBuilder().WithTitle("First Post").Build();
        var secondPost = new BlogPostBuilder().WithTitle("Second Post").Build();
        await Repository.StoreAsync(firstPost);
        await Repository.StoreAsync(secondPost);
        bookmarkService.GetBookmarkedPostIds().Returns(new List<string> { firstPost.Id, secondPost.Id });
        var undoStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var undoCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        bookmarkService.SetBookmark(Arg.Any<string>(), false).Returns(Task.CompletedTask);
        bookmarkService.SetBookmark(Arg.Any<string>(), true).Returns(_ =>
        {
            undoStarted.SetResult();
            return undoCompletion.Task;
        });
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => bookmarkService);
        var cut = ctx.Render<Blog.Web.Features.Bookmarks.Bookmarks>();
        cut.WaitForElement(".bookmark-row");

        // Act
        await cut.Find("button[aria-label='Remove bookmark: First Post']").ClickAsync();
        var undoTask = cut.Find(".undo-button").ClickAsync();
        await undoStarted.Task;
        await cut.Find("button[aria-label='Remove bookmark: Second Post']").ClickAsync();
        undoCompletion.SetResult();
        await undoTask;

        // Assert
        cut.FindAll(".row-title").Select(e => e.TextContent).ShouldContain("First Post");
        cut.Find(".undo-message").TextContent.ShouldContain("Second Post");
    }

    [Fact]
    public async Task ShouldFilterBookmarksBySearchTerm()
    {
        // Arrange
        using var ctx = new BunitContext();
        var bookmarkService = Substitute.For<IBookmarkService>();
        var blazorPost = new BlogPostBuilder().WithTitle("Blazor Tips").Build();
        var cssPost = new BlogPostBuilder().WithTitle("CSS Tricks").Build();
        await Repository.StoreAsync(blazorPost);
        await Repository.StoreAsync(cssPost);
        bookmarkService.GetBookmarkedPostIds().Returns(new List<string> { blazorPost.Id, cssPost.Id });
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => bookmarkService);
        var cut = ctx.Render<Blog.Web.Features.Bookmarks.Bookmarks>();
        cut.WaitForElement(".bookmark-row");

        // Act
        var searchInput = cut.Find("input[type=search]");
        await searchInput.InputAsync("Blazor ");
        searchInput.GetAttribute("value").ShouldBe("Blazor ");
        await searchInput.InputAsync("Blazor Tips");

        // Assert
        var rows = cut.FindComponents<BookmarkRow>();
        rows.ShouldHaveSingleItem();
        rows[0].Find(".row-title").TextContent.ShouldBe("Blazor Tips");
    }

    [Fact]
    public async Task ShouldSortBookmarksByTitle()
    {
        // Arrange
        using var ctx = new BunitContext();
        var bookmarkService = Substitute.For<IBookmarkService>();
        var first = new BlogPostBuilder().WithTitle("Zebra").WithUpdatedDate(new System.DateTime(2025, 1, 2)).Build();
        var second = new BlogPostBuilder().WithTitle("Apple").WithUpdatedDate(new System.DateTime(2025, 1, 1)).Build();
        await Repository.StoreAsync(first);
        await Repository.StoreAsync(second);
        bookmarkService.GetBookmarkedPostIds().Returns(new List<string> { first.Id, second.Id });
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => bookmarkService);
        var cut = ctx.Render<Blog.Web.Features.Bookmarks.Bookmarks>();
        cut.WaitForElement(".bookmark-row");
        cut.FindAll(".row-title").Select(e => e.TextContent).ShouldBe(["Zebra", "Apple"]);

        // Act
        await cut.FindAll(".segmented button").Single(s => s.TextContent == "Title").ClickAsync();

        // Assert
        cut.FindAll(".row-title").Select(e => e.TextContent).ShouldBe(["Apple", "Zebra"]);
    }
}
