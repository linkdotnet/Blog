using System.Threading.Tasks;
using Blazored.Toast.Services;
using Bunit;
using Bunit.TestDoubles;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.BlogPostEditor;

public class CreateNewBlogPostPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldSaveBlogPostOnSave()
    {
        using var ctx = new TestContext();
        var toastService = new Mock<IToastService>();
        ctx.AddTestAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => toastService.Object);
        ctx.ComponentFactories.AddStub<UploadFile>();
        ctx.Services.AddScoped(_ => Mock.Of<IFileProcessor>());
        using var cut = ctx.RenderComponent<CreateBlogPost>();
        var newBlogPost = cut.FindComponent<CreateNewBlogPost>();

        TriggerNewBlogPost(newBlogPost);

        var blogPostFromDb = await DbContext.BlogPosts.SingleOrDefaultAsync(t => t.Title == "My Title");
        blogPostFromDb.Should().NotBeNull();
        blogPostFromDb.ShortDescription.Should().Be("My short Description");
        toastService.Verify(t => t.ShowInfo("Created BlogPost My Title", string.Empty, null), Times.Once);
    }

    [Fact]
    public async Task ShouldSetContentFromFile()
    {
        using var ctx = new TestContext();
        const string contentFromFile = "content";
        ctx.AddTestAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());
        var args = SetupUploadFile(contentFromFile, ctx);
        var cut = ctx.RenderComponent<CreateNewBlogPost>();
        var uploadFile = cut.FindComponent<UploadFile>();

        await uploadFile.InvokeAsync(() => cut.FindComponent<InputFile>().Instance.OnChange.InvokeAsync(args));

        cut.Find("#content").TextContent.Should().Be(contentFromFile);
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

    private static InputFileChangeEventArgs SetupUploadFile(string contentFromFile, TestContext ctx)
    {
        var file = new Mock<IBrowserFile>();
        var fileProcessor = new Mock<IFileProcessor>();
        fileProcessor.Setup(f => f.GetContent(file.Object)).ReturnsAsync(contentFromFile);
        var args = new InputFileChangeEventArgs(new[]
        {
            file.Object,
        });
        ctx.Services.AddScoped(_ => fileProcessor.Object);
        ctx.JSInterop.SetupVoid(invocation => invocation.Identifier == "Blazor._internal.InputFile.init")
            .SetVoidResult();
        return args;
    }
}