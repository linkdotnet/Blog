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
        using var ctx = new BunitContext();
        ctx.JSInterop.SetupVoid("hljs.highlightAll");
        var toastService = Substitute.For<IToastService>();
        var blogPost = new BlogPostBuilder().WithTitle("Title").WithShortDescription("Sub").Build();
        await Repository.StoreAsync(blogPost);
        ctx.AddAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => toastService);
        ctx.ComponentFactories.AddStub<UploadFile>();
        using var cut = ctx.Render<UpdateBlogPostPage>(
            p => p.Add(s => s.BlogPostId, blogPost.Id));
        var newBlogPost = cut.FindComponent<CreateNewBlogPost>();

        TriggerUpdate(newBlogPost);

        var blogPostFromDb = await DbContext.BlogPosts.SingleOrDefaultAsync(t => t.Id == blogPost.Id);
        blogPostFromDb.Should().NotBeNull();
        blogPostFromDb.ShortDescription.Should().Be("My new Description");
        toastService.Received(1).ShowInfo("Updated BlogPost Title", null);
    }

    [Fact]
    public void ShouldThrowWhenNoIdProvided()
    {
        using var ctx = new BunitContext();
        ctx.AddAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());

        Action act = () => ctx.Render<UpdateBlogPostPage>(
            p => p.Add(s => s.BlogPostId, null));

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    private static void TriggerUpdate(RenderedFragment cut)
    {
        cut.Find("#short").Input("My new Description");

        cut.Find("form").Submit();
    }
}