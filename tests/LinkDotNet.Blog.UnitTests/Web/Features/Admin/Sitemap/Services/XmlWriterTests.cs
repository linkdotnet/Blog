using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Sitemap.Services;

public sealed class XmlWriterTests
{
    [Fact]
    public async Task ShouldWriteToFile()
    {
        var myObj = new MyObject { Property = "Prop" };

        var buffer = await new XmlWriter().WriteToBuffer(myObj);

        var serializer = new XmlSerializer(typeof(MyObject));
        await using var memoryStream = new MemoryStream(buffer);
        var myObject = serializer.Deserialize(memoryStream) as MyObject;
        myObject.ShouldNotBeNull();
    }

    public class MyObject
    {
        public required string Property { get; set; }
    }
}