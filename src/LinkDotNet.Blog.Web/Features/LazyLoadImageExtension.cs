using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace LinkDotNet.Blog.Web.Features;

internal sealed class LazyLoadImageExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.DocumentProcessed -= AddLazyLoading;
        pipeline.DocumentProcessed += AddLazyLoading;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    private static void AddLazyLoading(MarkdownDocument document)
    {
        foreach (var image in document.Descendants<LinkInline>())
        {
            if (image.IsImage)
            {
                image.GetAttributes().AddPropertyIfNotExist("loading", "lazy");
            }
        }
    }
}
