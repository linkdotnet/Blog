using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Features;

public static class MarkdownConverter
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
        .UseEmojiAndSmiley()
        .UseCitations()
        .UseBootstrap()
        .Build();

    public static MarkupString RenderMarkupString(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return default;
        }

        return (MarkupString)Markdown.ToHtml(content, MarkdownPipeline);
    }

    public static string RenderPlanString(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return default;
        }

        return Markdown.ToPlainText(markdown, MarkdownPipeline);
    }
}