using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using NCronJob;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared.Admin;

public class BlogPostAdminActionsTests : BunitContext
{
    public BlogPostAdminActionsTests()
    {
        Services.AddSingleton(Substitute.For<IRepository<BlogPost>>());
        Services.AddSingleton(Substitute.For<IToastService>());
        Services.AddSingleton(Substitute.For<IInstantJobRegistry>());
        AddAuthorization().SetAuthorized("s");
    }
    
    [Fact]
    public async Task ShouldDeleteBlogPostWhenOkClicked()
    {
        const string blogPostId = "2";
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        Services.AddSingleton(repositoryMock);
        
        var cut = Render<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));
        await cut.Find("#delete-blogpost").ClickAsync();

        await cut.Find("#ok").ClickAsync();

        await repositoryMock.Received(1).DeleteAsync(blogPostId);
    }

    [Fact]
    public async Task ShouldNotDeleteBlogPostWhenCancelClicked()
    {
        const string blogPostId = "2";
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        
        Services.AddSingleton(repositoryMock);
        var cut = Render<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));
        await cut.Find("#delete-blogpost").ClickAsync();

        await cut.Find("#cancel").ClickAsync();

        await repositoryMock.Received(0).DeleteAsync(blogPostId);
    }

    [Fact]
    public void ShouldGoToEditPageForEdit()
    {
        const string blogPostId = "2";
        
        var cut = Render<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));

        var anchor = cut.Find("#edit-blogpost") as IHtmlAnchorElement;
        anchor.ShouldNotBeNull();
        anchor.Href.ShouldEndWith($"update/{blogPostId}");
    }
}