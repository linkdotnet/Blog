using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

public sealed class XmlFileWriter : IXmlFileWriter
{
    public async Task WriteObjectToXmlFileAsync<T>(T objectToSave, string fileName)
    {
        await using var file = File.Create(fileName);
        await using var xmlWriter = XmlWriter.Create(file, new XmlWriterSettings { Indent = true, Async = true });
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(xmlWriter, objectToSave);
        xmlWriter.Close();
        file.Close();
    }
}