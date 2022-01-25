using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Shared.Services;

public interface IMarkerService
{
    Task<string> GetNewMarkdownForElementAsync(string elementId, string content, string fenceBegin, string fenceEnd);
}