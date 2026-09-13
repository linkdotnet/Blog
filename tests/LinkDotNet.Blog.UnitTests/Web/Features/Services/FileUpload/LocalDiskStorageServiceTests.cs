using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Services.FileUpload;
using Microsoft.AspNetCore.Hosting;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Services.FileUpload;

public sealed class LocalDiskStorageServiceTests : IDisposable
{
    private readonly string webRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    [Fact]
    public async Task ShouldWriteFileToUploadsFolderAndReturnRelativeUrl()
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(webRootPath);
        var sut = new LocalDiskStorageService(environment);
        using var content = new MemoryStream(Encoding.UTF8.GetBytes("content"));

        var url = await sut.UploadFileAsync("image.png", content, new UploadOptions());

        url.ShouldBe("/uploads/image.png");
        var writtenFile = Path.Combine(webRootPath, "uploads", "image.png");
        File.Exists(writtenFile).ShouldBeTrue();
        (await File.ReadAllTextAsync(writtenFile, Xunit.TestContext.Current.CancellationToken)).ShouldBe("content");
    }

    [Fact]
    public async Task ShouldSanitizeFileNameToPreventPathTraversal()
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(webRootPath);
        var sut = new LocalDiskStorageService(environment);
        using var content = new MemoryStream(Encoding.UTF8.GetBytes("content"));

        var url = await sut.UploadFileAsync("../../evil.png", content, new UploadOptions());

        url.ShouldBe("/uploads/evil.png");
        File.Exists(Path.Combine(webRootPath, "uploads", "evil.png")).ShouldBeTrue();
    }

    public void Dispose()
    {
        if (Directory.Exists(webRootPath))
        {
            Directory.Delete(webRootPath, recursive: true);
        }
    }
}
