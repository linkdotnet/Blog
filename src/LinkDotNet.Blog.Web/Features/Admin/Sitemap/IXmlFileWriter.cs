using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap;

public interface IXmlFileWriter
{
    Task WriteObjectToXmlFileAsync<T>(T objectToSave, string fileName);
}