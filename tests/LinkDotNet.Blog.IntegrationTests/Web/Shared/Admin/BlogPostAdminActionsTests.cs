using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared.Admin;

public class BlogPostAdminActionsTests
{
    [Fact]
    public async Task ShouldDeleteBlogPostWhenOkClicked()
    {
        const string blogPostId = "2";
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        using var ctx = new BunitContext();
        ctx.AddAuthorization().SetAuthorized("s");
        ctx.Services.AddSingleton(repositoryMock);
        ctx.Services.AddSingleton(Substitute.For<IToastService>());
        var cut = ctx.Render<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));
        cut.Find("#delete-blogpost").Click();

        cut.Find("#ok").Click();

        await repositoryMock.Received(1).DeleteAsync(blogPostId);
    }

    [Fact]
    public async Task ShouldNotDeleteBlogPostWhenCancelClicked()
    {
        const string blogPostId = "2";
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        using var ctx = new BunitContext();
        ctx.AddAuthorization().SetAuthorized("s");
        ctx.Services.AddSingleton(repositoryMock);
        ctx.Services.AddSingleton(Substitute.For<IToastService>());
        var cut = ctx.Render<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));
        cut.Find("#delete-blogpost").Click();

        cut.Find("#cancel").Click();

        await repositoryMock.Received(0).DeleteAsync(blogPostId);
    }

    [Fact]
    public void ShouldGoToEditPageOnEditClick()
    {
        const string blogPostId = "2";
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        using var ctx = new BunitContext();
        ctx.AddAuthorization().SetAuthorized("s");
        ctx.Services.AddSingleton(repositoryMock);
        ctx.Services.AddSingleton(Substitute.For<IToastService>());
        var navigationManager = ctx.Services.GetRequiredService<NavigationManager>();
        var cut = ctx.Render<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));

        cut.Find("#edit-blogpost").Click();

        navigationManager.Uri.Should().EndWith($"update/{blogPostId}");
    }
}