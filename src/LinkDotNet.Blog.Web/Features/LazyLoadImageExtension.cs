using System.Linq;
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
        pipeline.DocumentProcessed -= AddLoadingHints;
        pipeline.DocumentProcessed += AddLoadingHints;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    private static void AddLoadingHints(MarkdownDocument document)
    {
        var images = document.Descendants<LinkInline>().Where(link => link.IsImage).Select(link => link.GetAttributes()).ToList();
        foreach (var attributes in images)
        {
            attributes.AddPropertyIfNotExist("decoding", "async");
        }

        images.FirstOrDefault()?.AddPropertyIfNotExist("fetchpriority", "high");
        foreach (var attributes in images.Skip(1))
        {
            attributes.AddPropertyIfNotExist("loading", "lazy");
        }
    }
}
