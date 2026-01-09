using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities.Fakes;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        var templateRepository = Substitute.For<IRepository<BlogPostTemplate>>();
        templateRepository.GetAllAsync().Returns(PagedList<BlogPostTemplate>.Empty);
        ctx.Services.AddScoped(_ => templateRepository);

        var currentUserService = Substitute.For<ICurrentUserService>();
        currentUserService.GetDisplayNameAsync().Returns("Test Author");
        ctx.Services.AddScoped(_ => currentUserService);

        var options = Substitute.For<IOptions<ApplicationConfiguration>>();

        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = true,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        ctx.Services.AddScoped(_ => options);

        using var cut = ctx.Render<CreateBlogPost>();
        var newBlogPost = cut.FindComponent<CreateNewBlogPost>();

        TriggerNewBlogPost(newBlogPost);

        var blogPostFromDb = await DbContext.BlogPosts.SingleOrDefaultAsync(t => t.Title == "My Title", TestContext.Current.CancellationToken);
        blogPostFromDb.ShouldNotBeNull();
        blogPostFromDb.ShortDescription.ShouldBe("My short Description");
        blogPostFromDb.AuthorName.ShouldBe("Test Author");

        toastService.Received(1).ShowInfo("Created BlogPost My Title", null);
        instantRegistry.Received(1).RunInstantJob<SimilarBlogPostJob>(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ShouldSaveAuthorNameAsNullWhenMultiAuthorModeIsDisabled()
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

        var templateRepository = Substitute.For<IRepository<BlogPostTemplate>>();
        templateRepository.GetAllAsync().Returns(PagedList<BlogPostTemplate>.Empty);
        ctx.Services.AddScoped(_ => templateRepository);

        var currentUserService = Substitute.For<ICurrentUserService>();
        currentUserService.GetDisplayNameAsync().Returns("Test Author");
        ctx.Services.AddScoped(_ => currentUserService);

        var options = Substitute.For<IOptions<ApplicationConfiguration>>();

        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = false,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        ctx.Services.AddScoped(_ => options);

        using var cut = ctx.Render<CreateBlogPost>();
        var newBlogPost = cut.FindComponent<CreateNewBlogPost>();

        TriggerNewBlogPost(newBlogPost);

        var blogPostFromDb = await DbContext.BlogPosts.SingleOrDefaultAsync(t => t.Title == "My Title", TestContext.Current.CancellationToken);
        blogPostFromDb.ShouldNotBeNull();
        blogPostFromDb.AuthorName.ShouldBeNull();
    }

    [Fact]
    public async Task ShouldLoadTemplate()
    {
        await using var ctx = new BunitContext();
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        var toastService = Substitute.For<IToastService>();
        ctx.JSInterop.SetupVoid("hljs.highlightAll");
        ctx.AddAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => toastService);
        ctx.Services.AddScoped(_ => Substitute.For<IFileProcessor>());
        ctx.Services.AddScoped(_ => Substitute.For<IInstantJobRegistry>());
        ctx.Services.AddScoped(_ => Substitute.For<ICacheInvalidator>());
        var shortCodeRepository = Substitute.For<IRepository<ShortCode>>();
        shortCodeRepository.GetAllAsync().Returns(PagedList<ShortCode>.Empty);
        ctx.Services.AddScoped(_ => shortCodeRepository);

        var templateRepository = Substitute.For<IRepository<BlogPostTemplate>>();
        var template = BlogPostTemplate.Create("My Template", "Title", "Short", "Content");
        templateRepository.GetAllAsync().Returns(new PagedList<BlogPostTemplate>([template], 1, 1, 1));
        ctx.Services.AddScoped(_ => templateRepository);

        var currentUserService = Substitute.For<ICurrentUserService>();
        ctx.Services.AddScoped(_ => currentUserService);

        var options = Substitute.For<IOptions<ApplicationConfiguration>>();
        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = false,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });
        ctx.Services.AddScoped(_ => options);

        using var cut = ctx.Render<CreateNewBlogPost>(p => p.Add(s => s.Title, string.Empty));

        // Act
        var templateButton = cut.FindAll("button.dropdown-item").First(b => b.TextContent.Contains("My Template"));
        templateButton.Click();

        // Assert
        cut.Find("#title").Attributes["value"]!.Value.ShouldBe("Title");
        cut.Find("#short").TextContent.ShouldBe("Short");
        cut.Find("#content").TextContent.ShouldBe("Content");
    }

    private static void TriggerNewBlogPost(IRenderedComponent<CreateNewBlogPost> cut)
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
