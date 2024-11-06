using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Components;

public class UploadFileModalDialogTests : BunitContext
{
    [Fact]
    public async Task ShouldReturnFilenameAndSettingWhenSubmitting()
    {
        var cut = Render<UploadFileModalDialog>();
        var task = cut.InvokeAsync(() => cut.Instance.ShowAsync("Filename.png"));
        cut.Find("#cache").Change(false);
        
        await cut.Find("form").SubmitAsync();

        var result = await task;
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Filename.png");
        result.CacheMedia.ShouldBeFalse();
    }
    
    [Fact]
    public async Task ShouldReturnNullWhenAborted()
    {
        var cut = Render<UploadFileModalDialog>();
        var task = cut.InvokeAsync(() => cut.Instance.ShowAsync("Filename.png"));
        
        cut.Find("#abort").Click();

        var result = await task;
        result.ShouldBeNull();
    }
}
