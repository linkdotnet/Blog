using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace LinkDotNet.Blog.Web.Features;

internal sealed class ResponsiveTableExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer htmlRenderer)
        {
            return;
        }

        var originalTableRenderer = htmlRenderer.ObjectRenderers.FindExact<HtmlTableRenderer>();
        if (originalTableRenderer is null)
        {
            return;
        }

        htmlRenderer.ObjectRenderers.Remove(originalTableRenderer);
        htmlRenderer.ObjectRenderers.Add(new ResponsiveHtmlTableRenderer());
    }
}

internal sealed class ResponsiveHtmlTableRenderer : HtmlTableRenderer
{
    protected override void Write(HtmlRenderer renderer, Table table)
    {
        renderer.Write("""<div class="table-container">""");
        base.Write(renderer, table);
        renderer.EnsureLine();
        renderer.Write("</div>");
    }
}
