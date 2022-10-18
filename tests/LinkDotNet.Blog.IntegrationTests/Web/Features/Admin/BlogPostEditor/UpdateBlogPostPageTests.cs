using System;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.BlogPostEditor;

public class UpdateBlogPostPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldSaveBlogPostOnSave()
    {
        using var ctx = new TestContext();
        ctx.JSInterop.SetupVoid("hljs.highlightAll");
        var toastService = new Mock<IToastService>();
        var blogPost = new BlogPostBuilder().WithTitle("Title").WithShortDescription("Sub").Build();
        await Repository.StoreAsync(blogPost);
        ctx.AddTestAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => toastService.Object);
        ctx.ComponentFactories.AddStub<UploadFile>();
        using var cut = ctx.RenderComponent<UpdateBlogPostPage>(
            p => p.Add(s => s.BlogPostId, blogPost.Id));
        var newBlogPost = cut.FindComponent<CreateNewBlogPost>();

        TriggerUpdate(newBlogPost);

        var blogPostFromDb = await DbContext.BlogPosts.SingleOrDefaultAsync(t => t.Id == blogPost.Id);
        blogPostFromDb.Should().NotBeNull();
        blogPostFromDb.ShortDescription.Should().Be("My new Description");
        toastService.Verify(t => t.ShowInfo("Updated BlogPost Title", string.Empty, null), Times.Once);
    }

    [Fact]
    public void ShouldThrowWhenNoIdProvided()
    {
        using var ctx = new TestContext();
        ctx.AddTestAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());

        Action act = () => ctx.RenderComponent<UpdateBlogPostPage>(
            p => p.Add(s => s.BlogPostId, null));

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    private static void TriggerUpdate(IRenderedFragment cut)
    {
        cut.Find("#short").Input("My new Description");

        cut.Find("form").Submit();
    }
}