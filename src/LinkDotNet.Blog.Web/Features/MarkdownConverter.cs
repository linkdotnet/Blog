using System.Collections.Generic;
using System.Linq;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Features;

public static class MarkdownConverter
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
        .UseEmojiAndSmiley()
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

    public static IReadOnlyCollection<TocItem> GenerateToc(string markdownContent)
    {
        var document = Markdown.Parse(markdownContent, MarkdownPipeline);

        return document
            .Descendants<HeadingBlock>()
            .Where(h => h.Inline?.FirstChild is not null)
            .Select(heading => new TocItem { Level = heading.Level, Text = heading.Inline.FirstChild.ToString(), Id = heading.GetAttributes().Id })
            .ToArray();
    }
}

public class TocItem
{
    public int Level { get; set; }
    public string Text { get; set; }
    public string Id { get; set; }
}
