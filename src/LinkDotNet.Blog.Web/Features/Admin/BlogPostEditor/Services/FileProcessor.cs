using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

public sealed class FileProcessor : IFileProcessor
{
    public async Task<string> GetContentAsync(IBrowserFile file)
    {
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
