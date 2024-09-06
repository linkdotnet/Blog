using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BlogPostEditor.Services;

public class FileProcessorTests
{
    [Fact]
    public async Task ShouldProcessFileContent()
    {
        var browserFile = Substitute.For<IBrowserFile>();
        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        const string streamString = "Hello World";
        await writer.WriteAsync(streamString);
        await writer.FlushAsync();
        stream.Position = 0;
        browserFile.OpenReadStream(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(stream);

        var content = await new FileProcessor().GetContentAsync(browserFile);

        content.ShouldBe(streamString);
    }
}