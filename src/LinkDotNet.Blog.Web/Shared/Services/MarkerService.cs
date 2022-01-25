using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace LinkDotNet.Blog.Web.Shared.Services;

public class MarkerService : IMarkerService
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
            return string.Empty;
        }

        var beforeMarker = selectionRange.Start > 0 ? content[..selectionRange.Start] : string.Empty;
        var marker = content.Substring(selectionRange.Start, selectionRange.End - selectionRange.Start);
        var afterMarker = content.Substring(selectionRange.End, content.Length - selectionRange.End - 1);
        return beforeMarker + fenceBegin + marker + fenceEnd + afterMarker;
    }

    private sealed class SelectionRange
    {
        public int Start { get; set; }

        public int End { get; set; }
    }
}