using System.IO;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services.FileUpload;

public class NoopStorageService : IBlobUploadService
{
    public Task<string> UploadFileAsync(string fileName, Stream fileStream, UploadOptions options)
    {
        return Task.FromResult("No Storage Service was configured. Nothing was uploaded");
    }
}
