using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities.Fakes;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NCronJob;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.BlogPostEditor;

public class CreateNewBlogPostPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldSaveBlogPostOnSave()
    {
        await using var ctx = new BunitContext();
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        var toastService = Substitute.For<IToastService>();
        var instantRegistry = Substitute.For<IInstantJobRegistry>();
        ctx.JSInterop.SetupVoid("hljs.highlightAll");
        ctx.AddAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => toastService);
        ctx.Services.AddScoped(_ => Substitute.For<IFileProcessor>());
        ctx.Services.AddScoped(_ => instantRegistry);
        ctx.Services.AddScoped(_ => Substitute.For<ICacheInvalidator>());
        var shortCodeRepository = Substitute.For<IRepository<ShortCode>>();
        shortCodeRepository.GetAllAsync().Returns(PagedList<ShortCode>.Empty);
        ctx.Services.AddScoped(_ => shortCodeRepository);
        ctx.Services.AddScoped(_ => Substitute.For<IDraftService>());
        
        using var cut = ctx.Render<CreateBlogPost>();
        var newBlogPost = cut.FindComponent<CreateNewBlogPost>();

        TriggerNewBlogPost(newBlogPost);

        var blogPostFromDb = await DbContext.BlogPosts.SingleOrDefaultAsync(t => t.Title == "My Title", TestContext.Current.CancellationToken);
        blogPostFromDb.ShouldNotBeNull();
        blogPostFromDb.ShortDescription.ShouldBe("My short Description");
        toastService.Received(1).ShowInfo("Created BlogPost My Title", null);
        instantRegistry.Received(1).RunInstantJob<SimilarBlogPostJob>(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    private static void TriggerNewBlogPost(RenderedComponent<CreateNewBlogPost> cut)
    {
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#published").Change(false);
        cut.Find("#tags").Change("Tag1,Tag2,Tag3");

        cut.Find("form").Submit();
    }
}
