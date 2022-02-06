using System.IO;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Shared.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Services;

public class FileProcessorTests
{
    [Fact]
    public async Task ShouldProcessFileContent()
    {
        var browserFile = new Mock<IBrowserFile>();
        await using var stream = new MemoryStream();
        await using var writer = new StreamWriter(stream);
        const string streamString = "Hello World";
        await writer.WriteAsync(streamString);
        await writer.FlushAsync();
        stream.Position = 0;
        browserFile.Setup(b => b.OpenReadStream(It.IsAny<long>(), default))
            .Returns(stream);

        var content = await new FileProcessor().GetContent(browserFile.Object);

        content.Should().Be(streamString);
    }
}