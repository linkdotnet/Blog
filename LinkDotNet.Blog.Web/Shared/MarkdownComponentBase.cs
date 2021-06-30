using Markdig;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Shared
{
    public class MarkdownComponentBase : ComponentBase
    {
        private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseEmojiAndSmiley().Build();

        protected static MarkupString RenderMarkupString(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return default;
            }

            return (MarkupString)Markdown.ToHtml(content, MarkdownPipeline);
        }
    }
}