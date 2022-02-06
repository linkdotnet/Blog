using System;
using System.IO;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Shared.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Services;

public sealed class XmlFileWriterTests : IDisposable
{
    private const string OutputFilename = "somefile.txt";

    [Fact]
    public async Task ShouldWriteToFile()
    {
        var myObj = new MyObject { Property = "Prop" };

        await new XmlFileWriter().WriteObjectToXmlFileAsync(myObj, OutputFilename);

        var content = await File.ReadAllTextAsync(OutputFilename);
        content.Should().NotBeNull();
        content.Should().Contain("<MyObject");
        content.Should().Contain("<Property>Prop</Property>");
    }

    public void Dispose()
    {
        if (File.Exists(OutputFilename))
        {
            File.Delete(OutputFilename);
        }
    }

    public class MyObject
    {
        public string Property { get; set; }
    }
}
