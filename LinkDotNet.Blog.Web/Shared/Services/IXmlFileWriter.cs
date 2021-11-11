using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Shared.Services;

public interface IXmlFileWriter
{
    Task WriteObjectToXmlFileAsync<T>(T objectToSave, string fileName);
}
