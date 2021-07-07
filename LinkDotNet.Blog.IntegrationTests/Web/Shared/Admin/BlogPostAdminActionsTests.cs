using Blazored.Toast.Services;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared.Admin;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared.Admin
{
    public class BlogPostAdminActionsTests
    {
        [Fact]
        public void ShouldDeleteBlogPostWhenOkClicked()
        {
            const string blogPostId = "2";
            var repositoryMock = new Mock<IRepository>();
            using var ctx = new TestContext();
            ctx.AddTestAuthorization().SetAuthorized("s");
            ctx.Services.AddSingleton(repositoryMock.Object);
            ctx.Services.AddSingleton(new Mock<IToastService>().Object);
            var cut = ctx.RenderComponent<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));
            cut.Find("#delete-blogpost").Click();

            cut.Find("#ok").Click();

            repositoryMock.Verify(r => r.DeleteAsync(blogPostId), Times.Once);
        }

        [Fact]
        public void ShouldNotDeleteBlogPostWhenCancelClicked()
        {
            const string blogPostId = "2";
            var repositoryMock = new Mock<IRepository>();
            using var ctx = new TestContext();
            ctx.AddTestAuthorization().SetAuthorized("s");
            ctx.Services.AddSingleton(repositoryMock.Object);
            ctx.Services.AddSingleton(new Mock<IToastService>().Object);
            var cut = ctx.RenderComponent<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));
            cut.Find("#delete-blogpost").Click();

            cut.Find("#cancel").Click();

            repositoryMock.Verify(r => r.DeleteAsync(blogPostId), Times.Never);
        }

        [Fact]
        public void ShouldGoToEditPageOnEditClick()
        {
            const string blogPostId = "2";
            var repositoryMock = new Mock<IRepository>();
            using var ctx = new TestContext();
            ctx.AddTestAuthorization().SetAuthorized("s");
            ctx.Services.AddSingleton(repositoryMock.Object);
            ctx.Services.AddSingleton(new Mock<IToastService>().Object);
            var navigationManager = ctx.Services.GetRequiredService<NavigationManager>();
            var cut = ctx.RenderComponent<BlogPostAdminActions>(s => s.Add(p => p.BlogPostId, blogPostId));

            cut.Find("#edit-blogpost").Click();

            navigationManager.Uri.Should().EndWith($"update/{blogPostId}");
        }
    }
}