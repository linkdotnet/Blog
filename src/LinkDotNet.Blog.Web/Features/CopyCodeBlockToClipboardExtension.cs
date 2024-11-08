using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace LinkDotNet.Blog.Web.Features;

internal static class MarkdownPipelineBuilderExtensions
{
    public static MarkdownPipelineBuilder UseCopyCodeBlock(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.Add(new CopyCodeBlockToClipboardExtension());
        return pipeline;
    }
}

internal sealed class CopyCodeBlockToClipboardExtension : IMarkdownExtension
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

        var originalCodeBlockRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
        if (originalCodeBlockRenderer is null)
        {
            return;
        }

        htmlRenderer.ObjectRenderers.Remove(originalCodeBlockRenderer);
        htmlRenderer.ObjectRenderers.Add(new CustomCodeBlockRenderer());
    }
}

internal sealed class CustomCodeBlockRenderer : CodeBlockRenderer
{
    protected override void Write(HtmlRenderer renderer, CodeBlock obj)
    {
        renderer.Write("""<div class="position-relative">""");
        renderer.Write("""
                       <button class="btn btn-sm position-absolute top-0 end-0 m-2 border border-primary text-primary copy-btn"
                               type="button"
                               onclick="navigator.clipboard.writeText(this.parentElement.querySelector('pre code').textContent)">
                               <i class="copy"></i>
                       </button>
                       """);
        base.Write(renderer, obj);
        renderer.Write("</div>");
    }
}
