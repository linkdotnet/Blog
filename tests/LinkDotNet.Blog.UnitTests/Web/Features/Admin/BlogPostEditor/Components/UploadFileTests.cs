using System.Collections.Generic;
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
        var file = new Mock<IBrowserFile>();
        var fileProcessor = new Mock<IFileProcessor>();
        fileProcessor.Setup(f => f.GetContentAsync(file.Object)).ReturnsAsync(content);
        var args = new InputFileChangeEventArgs(new[]
        {
            file.Object,
        });
        Services.AddScoped(_ => fileProcessor.Object);
        var cut = RenderComponent<UploadFile>(
            s => s.Add(p => p.OnFileUploaded, f => invokedContent = f));
        var inputComponent = cut.FindComponent<InputFile>().Instance;

        await cut.InvokeAsync(() => inputComponent.OnChange.InvokeAsync(args));

        invokedContent.Should().Be(content);
    }

    [Fact]
    public void ShouldPutIdAndClassOnItems()
    {
        Services.AddScoped(_ => Mock.Of<IFileProcessor>());
        var attributes = new Dictionary<string, object>
        {
            { "class", "some-class" },
            { "id", "some-id" },
        };

        var cut = RenderComponent<UploadFile>(p => p.Add(s => s.AdditionalAttributes, attributes));

        cut.FindAll(".some-class").Should().NotBeEmpty();
        cut.FindAll("#some-id").Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldIndicateDragAndDropBehavior()
    {
        Services.AddScoped(_ => Mock.Of<IFileProcessor>());
        var cut = RenderComponent<UploadFile>();

        cut.Find("input").DragEnter();

        cut.Find(".can-drop").Should().NotBeNull();
    }

    [Fact]
    public void ShouldRemoveDragAndDropBehaviorWhenOutside()
    {
        Services.AddScoped(_ => Mock.Of<IFileProcessor>());
        var cut = RenderComponent<UploadFile>();
        cut.Find("input").DragEnter();

        cut.Find("input").DragLeave();

        cut.FindAll(".can-drop").Should().BeEmpty();
    }
}