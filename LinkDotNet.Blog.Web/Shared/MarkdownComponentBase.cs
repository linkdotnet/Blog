using Markdig;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Shared
{
    public abstract class MarkdownComponentBase : ComponentBase
    {
        private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseEmojiAndSmiley()
            .UseCitations()
            .UseBootstrap()
            .Build();

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