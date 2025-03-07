using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Bookmarks;
using LinkDotNet.Blog.Web.Features.Bookmarks.Components;
using LinkDotNet.Blog.Web.Features.Components;
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
        cut.WaitForElement(".blog-card");
        var blogPosts = cut.FindComponents<ShortBlogPost>();
        
        blogPosts.ShouldHaveSingleItem();
        blogPosts[0].Find(".description h1").TextContent.ShouldBe("Bookmarked Post");
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
        bookmarkService.IsBookmarked(bookmarkedBlogPost.Id).Returns(true);
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => bookmarkService);
        
        // Act
        var cut = ctx.Render<Blog.Web.Features.Bookmarks.Bookmarks>();
        cut.WaitForElement(".blog-card");
        
        // Find and click the bookmark button
        var bookmarkButton = cut.FindComponent<BookmarkButton>().Find("button");
        bookmarkButton.Click();
        
        // Assert
        await bookmarkService.Received(1).SetBookmark(bookmarkedBlogPost.Id, false);
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
        cut.WaitForElement("p");
        cut.Find("p").TextContent.ShouldBe("You have no bookmarks");
        cut.FindComponents<ShortBlogPost>().Count.ShouldBe(0);
    }
}
