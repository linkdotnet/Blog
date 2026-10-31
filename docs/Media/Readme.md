# Media Upload

The blog supports uploading media assets (images, etc.) while writing blog posts, either to Azure Blob Storage or to local disk.

## Configuration

The following settings in `appsettings.json` control media upload functionality:

```json
{
  "ImageStorageProvider": "Azure",
  "ImageStorage": {
    "AuthenticationMode": "Default",
    "ConnectionString": "",
    "ServiceUrl": "", 
    "ContainerName": ""
  }
}
```

| Property                        | Type   | Description                                                                                                          |
| ------------------------------- | ------ | -------------------------------------------------------------------------------------------------------------------- |
| ImageStorageProvider            | string | `Azure` for Azure Blob Storage, or `Local` to store files on disk under `wwwroot/uploads` (useful for self-hosting without a cloud provider). Any other value disables uploads. |
| AuthenticationMode | string | Authentication method - either `Default` for Microsoft Entra ID or `ConnectionString` for connection string auth |
| ConnectionString   | string | Azure Storage connection string (only used and mandatory when AuthenticationMode is `ConnectionString`)                          |
| ServiceUrl         | string | Azure Blob Storage service URL (only used and mandatory when AuthenticationMode is `Default`)                                    |
| ContainerName      | string | Name of the Azure Storage container to store uploaded files. It can be nested containers as well ``path/to/upload``                                                       |
| CdnEndpoint | string | Optional CDN endpoint to use for uploaded images. If set, the blog will return this URL instead of the storage account URL for uploaded assets. |

### Local storage and serving uploaded files

With `ImageStorageProvider` set to `Local`, files are written to `wwwroot/uploads`. The app currently maps build-time static assets; runtime-created uploads are not served automatically by that mapping. To make local uploads accessible, configure ASP.NET Core static-file middleware to serve the uploads directory and persist that directory across container replacements. Alternatively, use Azure Blob Storage.

## Authentication Methods

### Default Authentication 
Uses Microsoft Entra ID (formerly Azure AD) managed identity or default credentials. This requires:

1. Setting up proper RBAC permissions in Azure
2. Configuring the ServiceUrl to point to your storage account
3. No sensitive credentials needed in config

### Connection String Authentication
Uses a storage account connection string for authentication:

1. Get connection string from Azure Portal
2. Add it to the ConnectionString setting
3. No additional Azure setup required

## Usage

1. Start writing a blog post in the markdown editor
2. Use the image toolbar button, or paste an image directly from the clipboard (e.g. a screenshot)
3. A dialog appears asking for:
   - File name (can include subdirectories)
   - Whether to enable browser caching via `Cache-Control` headers (currently set to one week)
4. The image gets uploaded and a markdown image link is inserted

## Performance Note

Use a CDN endpoint for uploaded images to improve performance. This can be set in the `CdnEndpoint` setting. A CDN can provide HTTP/2 and other performance benefits.