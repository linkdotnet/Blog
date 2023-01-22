using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

public interface IFileProcessor
{
    Task<string> GetContentAsync(IBrowserFile file);
}
