using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BlogPostEditor.Components;

public class UploadFileTests : TestContext
{
    public UploadFileTests()
    {
        JSInterop.SetupVoid(invocation => invocation.Identifier == "Blazor._internal.InputFile.init")
            .SetVoidResult();
    }

    [Fact]
    public async Task ShouldCallProcessorWhenFilesUploaded()
    {
        const string content = "Test";
        var invokedContent = string.Empty;
        var file = Substitute.For<IBrowserFile>();
        var fileProcessor = Substitute.For<IFileProcessor>();
        fileProcessor.GetContentAsync(file).Returns(content);
        var args = new InputFileChangeEventArgs(new[]
        {
            file,
        });
        Services.AddScoped(_ => fileProcessor);
        var cut = RenderComponent<UploadFile>(
            s => s.Add(p => p.OnFileUploaded, f => invokedContent = f));
        var inputComponent = cut.FindComponent<InputFile>().Instance;

        await cut.InvokeAsync(() => inputComponent.OnChange.InvokeAsync(args));

        invokedContent.Should().Be(content);
    }

    [Fact]
    public void ShouldIndicateDragAndDropBehavior()
    {
        Services.AddScoped(_ => Substitute.For<IFileProcessor>());
        var cut = RenderComponent<UploadFile>();

        cut.Find("input").DragEnter();

        cut.Find(".can-drop").Should().NotBeNull();
    }

    [Fact]
    public void ShouldRemoveDragAndDropBehaviorWhenOutside()
    {
        Services.AddScoped(_ => Substitute.For<IFileProcessor>());
        var cut = RenderComponent<UploadFile>();
        cut.Find("input").DragEnter();

        cut.Find("input").DragLeave();

        cut.FindAll(".can-drop").Should().BeEmpty();
    }
}