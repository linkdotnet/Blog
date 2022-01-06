using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace LinkDotNet.Blog.Web.Shared.Services;

public interface IFileProcessor
{
    Task<string> GetContent(IBrowserFile file);
}