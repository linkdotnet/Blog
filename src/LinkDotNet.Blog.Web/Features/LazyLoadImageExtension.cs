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
        pipeline.DocumentProcessed -= AddLazyLoading;
        pipeline.DocumentProcessed += AddLazyLoading;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
    }

    private static void AddLazyLoading(MarkdownDocument document)
    {
        var allImagesBesidesFirst = document.Descendants<LinkInline>().Where(link => link.IsImage).Skip(1);
        foreach (var image in allImagesBesidesFirst)
        {
            image.GetAttributes().AddPropertyIfNotExist("loading", "lazy");
        }
    }
}
