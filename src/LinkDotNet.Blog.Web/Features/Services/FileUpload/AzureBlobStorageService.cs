using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        var (rootContainer, subContainer) = SplitContainerName(containerName);
        var blobContainerClient = client.GetBlobContainerClient(rootContainer);
        var blobClient = blobContainerClient.GetBlobClient($"{subContainer}/{fileName}");

        var blobOptions = new BlobUploadOptions();
        if (options.SetCacheControlHeader)
        {
            blobOptions.HttpHeaders = new BlobHttpHeaders
            {
                CacheControl = "public, max-age=604800"
            };
        }

        await blobClient.UploadAsync(fileStream, blobOptions);
        return GetAssetUrl(blobClient.Uri.ToString(), azureBlobStorageConfiguration.Value);
    }

    private static (string rootContainer, string subContainer) SplitContainerName(string containerName)
    {
        var containerNames = containerName.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (containerNames.Length == 0)
            return (string.Empty, string.Empty);

        var rootContainer = containerNames[0];
        var subContainer = string.Join("/", containerNames.Skip(1));
        return (rootContainer, subContainer);
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

    private static string GetAssetUrl(string blobUrl, UploadConfiguration config)
    {
        if (!config.IsCdnEnabled)
        {
            return blobUrl;
        }

        var cdnEndpoint = config.CdnEndpoint!.TrimEnd('/');
        var blobUri = new Uri(blobUrl);
        var path = blobUri.AbsolutePath;

        return $"{cdnEndpoint}{path}";
    }
}
