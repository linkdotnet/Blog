using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;

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
            .Select(heading => new TocItem { Level = heading.Level, Text = InlineToString(heading.Inline), Id = heading.GetAttributes().Id })
            .ToArray();
    }

    private static string InlineToString(ContainerInline inline)
    {
        var sb = new StringBuilder();
        var current = inline.FirstChild;
        while (current is not null)
        {
            if (current is CodeInline cd)
            {
                sb.Append(cd.Content);
            }
            else
            {
                sb.Append(current);
            }

            current = current.NextSibling;
        }

        return sb.ToString();
    }
}

public class TocItem
{
    public int Level { get; set; }
    public string Text { get; set; }
    public string Id { get; set; }
}
