using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

public interface IXmlWriter
{
    Task<byte[]> WriteToBuffer<T>(T objectToSave);
}
