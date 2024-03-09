using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
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
        using var ctx = new BunitContext();
        var toastService = Substitute.For<IToastService>();
        ctx.JSInterop.SetupVoid("hljs.highlightAll");
        ctx.AddAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => toastService);
        ctx.ComponentFactories.AddStub<UploadFile>();
        ctx.Services.AddScoped(_ => Substitute.For<IFileProcessor>());
        using var cut = ctx.Render<CreateBlogPost>();
        var newBlogPost = cut.FindComponent<CreateNewBlogPost>();

        TriggerNewBlogPost(newBlogPost);

        var blogPostFromDb = await DbContext.BlogPosts.SingleOrDefaultAsync(t => t.Title == "My Title");
        blogPostFromDb.Should().NotBeNull();
        blogPostFromDb.ShortDescription.Should().Be("My short Description");
        toastService.Received(1).ShowInfo("Created BlogPost My Title", null);
    }

    [Fact]
    public async Task ShouldSetContentFromFile()
    {
        using var ctx = new BunitContext();
        const string contentFromFile = "content";
        ctx.JSInterop.SetupVoid("hljs.highlightAll");
        ctx.AddAuthorization().SetAuthorized("some username");
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        var args = SetupUploadFile(contentFromFile, ctx);
        var cut = ctx.Render<CreateNewBlogPost>();
        var uploadFile = cut.FindComponent<UploadFile>();

        await uploadFile.InvokeAsync(() => cut.FindComponent<InputFile>().Instance.OnChange.InvokeAsync(args));

        cut.Find("#content").TextContent.Should().Be(contentFromFile);
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

    private static InputFileChangeEventArgs SetupUploadFile(string contentFromFile, BunitContext ctx)
    {
        var file = Substitute.For<IBrowserFile>();
        var fileProcessor = Substitute.For<IFileProcessor>();
        fileProcessor.GetContentAsync(file).Returns(contentFromFile);
        var args = new InputFileChangeEventArgs(new[]
        {
            file,
        });
        ctx.Services.AddScoped(_ => fileProcessor);
        ctx.JSInterop.SetupVoid(invocation => invocation.Identifier == "Blazor._internal.InputFile.init")
            .SetVoidResult();
        return args;
    }
}
