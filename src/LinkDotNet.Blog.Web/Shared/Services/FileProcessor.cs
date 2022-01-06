using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace LinkDotNet.Blog.Web.Shared.Services;

public class FileProcessor : IFileProcessor
{
    public async Task<string> GetContent(IBrowserFile file)
    {
        await using var stream = file.OpenReadStream();
        var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}