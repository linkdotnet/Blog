using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.AspNetCore.Components;
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
        cut.Find("#delete-blogpost").Click();

        cut.Find("#ok").Click();

        await repositoryMock.Received(1).DeleteAsync(blogPostId);
    }

    [Fact]
    public async Task ShouldNotDeleteBlogPostWhenCancelClicked()
    {
        const string blogPostId = "2";
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        
        Services.AddSingleton(repositoryMock);
        var cut = Render<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));
        cut.Find("#delete-blogpost").Click();

        cut.Find("#cancel").Click();

        await repositoryMock.Received(0).DeleteAsync(blogPostId);
    }

    [Fact]
    public void ShouldGoToEditPageForEdit()
    {
        const string blogPostId = "2";
        
        var cut = Render<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));

        var anchor = cut.Find("#edit-blogpost") as IHtmlAnchorElement;
        anchor.Should().NotBeNull();
        anchor.Href.Should().EndWith($"update/{blogPostId}");
    }
}