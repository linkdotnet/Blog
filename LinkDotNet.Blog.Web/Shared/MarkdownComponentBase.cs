using Markdig;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Shared
{
    public class MarkdownComponentBase : ComponentBase
    {
        protected static MarkupString RenderMarkupString(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return default;
            }
            
            var markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseEmojiAndSmiley().Build();

            return (MarkupString) Markdown.ToHtml(content, markdownPipeline);
        }
    }
}