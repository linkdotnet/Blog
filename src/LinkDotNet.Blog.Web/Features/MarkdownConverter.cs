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

    public static MarkupString ToMarkupString(string markdown)
    {
        return string.IsNullOrEmpty(markdown)
            ? default
            : (MarkupString)Markdown.ToHtml(markdown, MarkdownPipeline);
    }

    public static string ToPlainString(string markdown)
    {
        return string.IsNullOrEmpty(markdown)
            ? default
            : Markdown.ToPlainText(markdown, MarkdownPipeline).TrimEnd('\r', '\n');
    }
}