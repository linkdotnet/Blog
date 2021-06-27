using Markdig;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Shared
{
    public class MarkdownComponentBase : ComponentBase
    {
        protected MarkupString RenderMarkupString(string content)
        {
            var p = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseEmojiAndSmiley().Build();

            return (MarkupString) Markdown.ToHtml(content, p);
        }
    }
}