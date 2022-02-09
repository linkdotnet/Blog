using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Features;

public abstract class MarkdownComponentBase : ComponentBase
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
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