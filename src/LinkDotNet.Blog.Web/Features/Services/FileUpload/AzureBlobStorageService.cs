using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.Features.Services.FileUpload;

public class AzureBlobStorageService : IBlobUploadService
{
    private readonly IOptions<UploadConfiguration> azureBlobStorageConfiguration;

    public AzureBlobStorageService(IOptions<UploadConfiguration> azureBlobStorageConfiguration)
    {
        this.azureBlobStorageConfiguration = azureBlobStorageConfiguration;
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream, UploadOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var containerName = azureBlobStorageConfiguration.Value.ContainerName;
        var client = CreateClient(azureBlobStorageConfiguration.Value);
        var blobContainerClient = client.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        var blobOptions = new BlobUploadOptions();
        if (options.SetCacheControlHeader)
        {
            blobOptions.HttpHeaders = new BlobHttpHeaders
            {
                CacheControl = "public, max-age=604800"
            };
        }

        await blobClient.UploadAsync(fileStream, blobOptions);
        return blobClient.Uri.AbsoluteUri;
    }

    private static BlobServiceClient CreateClient(UploadConfiguration configuration)
    {
        if (configuration.AuthenticationMode == AuthenticationMode.ConnectionString.Key)
        {
            var connectionString = configuration.ConnectionString
                                   ?? throw new InvalidOperationException("ConnectionString must be set when using ConnectionString authentication mode");
            return new BlobServiceClient(connectionString);
        }

        var serviceUrl = configuration.ServiceUrl
                         ?? throw new InvalidOperationException("ServiceUrl must be set when using Default authentication mode");

        return new BlobServiceClient(new Uri(serviceUrl), new DefaultAzureCredential());
    }
}
