using System.IO;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services.FileUpload;

public interface IBlobUploadService
{
    public Task<string> UploadFileAsync(string fileName, Stream fileStream, UploadOptions options);
}
