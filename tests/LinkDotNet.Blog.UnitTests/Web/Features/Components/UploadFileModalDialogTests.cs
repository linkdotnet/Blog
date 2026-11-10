using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.TestUtilities;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Components;

public class UploadFileModalDialogTests : BunitContext
{
    public UploadFileModalDialogTests()
    {
        JSInterop.SetupNativeDialog();
    }

    [Fact]
    public async Task ShouldReturnFilenameAndSettingWhenSubmitting()
    {
        var cut = Render<UploadFileModalDialog>();
        var task = cut.InvokeAsync(() => cut.Instance.ShowAsync(["Filename.png"]));
        cut.Find("#cache").Change(false);

        await cut.Find("form").SubmitAsync();

        var result = await task;
        result.ShouldNotBeNull();
        result.GetPaths().ShouldBe(["Filename.png"]);
        result.CacheMedia.ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldPrefixEveryFileWithTheSharedFolder()
    {
        var cut = Render<UploadFileModalDialog>();
        var task = cut.InvokeAsync(() => cut.Instance.ShowAsync(["a.png", "b.png"]));
        cut.Find("#upload-folder").Input("/20261110_Something/ ");
        cut.FindAll(".file-name")[1].Input("renamed.png");

        await cut.Find("form").SubmitAsync();

        var result = await task;
        result.ShouldNotBeNull();
        result.GetPaths().ShouldBe(["20261110_Something/a.png", "20261110_Something/renamed.png"]);
    }

    [Fact]
    public async Task ShouldNotSubmitWhenAFileNameIsBlank()
    {
        var cut = Render<UploadFileModalDialog>();
        var task = cut.InvokeAsync(() => cut.Instance.ShowAsync(["a.png", "b.png"]));
        cut.FindAll(".file-name")[0].Input(" ");

        await cut.Find("form").SubmitAsync();

        task.IsCompleted.ShouldBeFalse();
        cut.Find("button[type=submit]").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnNullWhenAborted()
    {
        var cut = Render<UploadFileModalDialog>();
        var task = cut.InvokeAsync(() => cut.Instance.ShowAsync(["Filename.png"]));

        await cut.Find("#abort").ClickAsync();

        var result = await task;
        result.ShouldBeNull();
    }
}
