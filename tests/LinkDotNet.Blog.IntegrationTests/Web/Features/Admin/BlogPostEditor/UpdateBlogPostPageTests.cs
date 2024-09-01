using System;
using System.Threading;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.TestUtilities.Fakes;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NCronJob;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.BlogPostEditor;

public class UpdateBlogPostPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldSaveBlogPostOnSave()
    {
        await using var ctx = new BunitContext();
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        ctx.JSInterop.SetupVoid("hljs.highlightAll");
        var toastService = Substitute.For<IToastService>();
        ctx.Services.AddScoped(_ => Substitute.For<ICacheInvalidator>());
        var instantRegistry = Substitute.For<IInstantJobRegistry>();
        var blogPost = new BlogPostBuilder().WithTitle("Title").WithShortDescription("Sub").Build();
        await Repository.StoreAsync(blogPost);
        ctx.AddAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => toastService);
        ctx.Services.AddScoped(_ => instantRegistry);
        using var cut = ctx.Render<UpdateBlogPostPage>(
            p => p.Add(s => s.BlogPostId, blogPost.Id));
        var newBlogPost = cut.FindComponent<CreateNewBlogPost>();

        TriggerUpdate(newBlogPost);

        var blogPostFromDb = await DbContext.BlogPosts.SingleOrDefaultAsync(t => t.Id == blogPost.Id);
        blogPostFromDb.Should().NotBeNull();
        blogPostFromDb.ShortDescription.Should().Be("My new Description");
        toastService.Received(1).ShowInfo("Updated BlogPost Title", null);
        instantRegistry.Received(1).RunInstantJob<SimilarBlogPostJob>(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void ShouldThrowWhenNoIdProvided()
    {
        using var ctx = new BunitContext();
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        ctx.AddAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        ctx.Services.AddScoped(_ => Substitute.For<ICacheInvalidator>());

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