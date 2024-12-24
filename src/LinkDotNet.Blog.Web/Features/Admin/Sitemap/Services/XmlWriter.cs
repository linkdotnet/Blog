using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

public sealed class XmlWriter : IXmlWriter
{
    public async Task<byte[]> WriteToBuffer<T>(T objectToSave)
    {
        await using var memoryStream = new MemoryStream();
        await using var xmlWriter = System.Xml.XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = true, Async = true });
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(xmlWriter, objectToSave);
        xmlWriter.Close();
        return memoryStream.ToArray();
    }
}
