using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace LinkDotNet.Blog.Web.Features.Services.FileUpload;

public class LocalDiskStorageService : IBlobUploadService
{
    private const string UploadsFolder = "uploads";
    private readonly IWebHostEnvironment environment;

    public LocalDiskStorageService(IWebHostEnvironment environment)
    {
        this.environment = environment;
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream, UploadOptions options)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        var directory = Path.Combine(environment.WebRootPath, UploadsFolder);
        Directory.CreateDirectory(directory);

        var safeFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(directory, safeFileName);

        await using var fileOnDisk = File.Create(filePath);
        await fileStream.CopyToAsync(fileOnDisk);

        return $"/{UploadsFolder}/{safeFileName}";
    }
}
