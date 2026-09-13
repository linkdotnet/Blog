using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace LinkDotNet.Blog.Web.Features;

internal static class MarkdownPipelineBuilderExtensions
{
    public static MarkdownPipelineBuilder UseCopyCodeBlock(this MarkdownPipelineBuilder pipeline, bool showLanguage)
    {
        pipeline.Extensions.Add(new CopyCodeBlockToClipboardExtension(showLanguage));
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseCallouts(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.Add(new CalloutExtension());
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseLazyLoadImages(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.Add(new LazyLoadImageExtension());
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseExternalLinks(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.Add(new ExternalLinkExtension());
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseResponsiveTables(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.Add(new ResponsiveTableExtension());
        return pipeline;
    }
}

internal sealed class CopyCodeBlockToClipboardExtension(bool showLanguage) : IMarkdownExtension
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
        htmlRenderer.ObjectRenderers.Add(new CustomCodeBlockRenderer(showLanguage));
    }
}

internal sealed class CustomCodeBlockRenderer(bool showLanguage) : CodeBlockRenderer
{
    protected override void Write(HtmlRenderer renderer, CodeBlock obj)
    {
        renderer.Write("""<div class="code-block"><div class="code-block-header d-flex align-items-center">""");
        if (showLanguage && obj is FencedCodeBlock { Info.Length: > 0 } fenced)
        {
            renderer.Write("""<span class="code-block-lang">""").WriteEscape(fenced.Info).Write("</span>");
        }
        renderer.Write("""
                       <button class="btn btn-sm py-0 ms-auto border border-primary text-primary copy-btn"
                               type="button"
                               onclick="navigator.clipboard.writeText(this.closest('.code-block').querySelector('pre code').textContent)">
                               <i class="copy"></i>
                       </button>
                       </div>
                       """);
        base.Write(renderer, obj);
        renderer.Write("</div>");
    }
}
