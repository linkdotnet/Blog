using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace LinkDotNet.Blog.Web.Shared.Services;

public partial class MarkerService : IMarkerService
{
    private readonly IJSRuntime jsRuntime;

    public MarkerService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task<string> GetNewMarkdownForElementAsync(string elementId, string content, string fenceBegin, string fenceEnd)
    {
        var selectionRange = await jsRuntime.InvokeAsync<SelectionRange>("getSelectionFromElement", elementId);
        if (selectionRange.Start == selectionRange.End)
        {
            return content;
        }

        var beforeMarker = selectionRange.Start > 0 ? content[..selectionRange.Start] : string.Empty;
        var marker = content[selectionRange.Start..selectionRange.End];
        var afterMarker = content[selectionRange.End..];
        return beforeMarker + fenceBegin + marker + fenceEnd + afterMarker;
    }
}