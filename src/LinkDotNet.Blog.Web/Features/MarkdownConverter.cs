using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Features;

public static class MarkdownConverter
{
    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseAutoIdentifiers(AutoIdentifierOptions.GitHub)
        .UseEmojiAndSmiley()
        .UseBootstrap()
        .UseCopyCodeBlock()
        .UseCallouts()
        .UseLazyLoadImages()
        .UseExternalLinks()
        .UseResponsiveTables()
        .Build();

    public static MarkupString ToMarkupString(string markdown)
    {
        return string.IsNullOrEmpty(markdown)
            ? default
            : (MarkupString)Markdown.ToHtml(markdown, MarkdownPipeline);
    }

    public static MarkupString ToMarkupStringWithHeadingAnchors(string markdown, string currentUri)
    {
        ArgumentNullException.ThrowIfNull(currentUri);

        if (string.IsNullOrEmpty(markdown))
        {
            return default;
        }

        var document = Markdown.Parse(markdown, MarkdownPipeline);
        // Blazor resolves a bare "#anchor" against the base href (the home page), so the link needs the full page URL
        var pageUri = currentUri.Split('#')[0];

        foreach (var heading in document.Descendants<HeadingBlock>().ToList())
        {
            var id = heading.GetAttributes().Id;
            if (heading.Inline is null || string.IsNullOrEmpty(id))
            {
                continue;
            }

            var anchor = new LinkInline($"{pageUri}#{id}", string.Empty);
            anchor.AppendChild(new LiteralInline("#"));
            var attributes = anchor.GetAttributes();
            attributes.AddClass("heading-anchor ms-2 link-secondary link-opacity-25 link-opacity-100-hover link-underline-opacity-0");
            attributes.AddProperty("title", "Copy link to this section");
            attributes.AddProperty("onclick", "navigator.clipboard?.writeText(this.href)");
            attributes.AddProperty("aria-label", "Link to this section");
            heading.Inline.AppendChild(anchor);
        }

        return (MarkupString)document.ToHtml(MarkdownPipeline);
    }

    public static string? ToPlainString(string markdown)
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
            .Select(heading => new TocItem
            {
                Level = heading.Level,
                Text = InlineToString(heading.Inline),
                Id = heading.GetAttributes().Id!
            })
            .ToArray();
    }

    private static string InlineToString(ContainerInline? inline)
    {
        var sb = new StringBuilder();
        ProcessInlineDelegate(inline, sb);
        return sb.ToString();

        static void ProcessInlineDelegate(Inline? inline, StringBuilder stringBuilder)
        {
            if (inline is null)
            {
                return;
            }

            var current = inline;
            while (current is not null)
            {
                switch (current)
                {
                    case CodeInline cd:
                        stringBuilder.Append(cd.Content);
                        break;
                    case LinkInline link:
                        ProcessInlineDelegate(link.FirstChild, stringBuilder);
                        break;
                    case EmphasisInline em:
                        ProcessInlineDelegate(em.FirstChild, stringBuilder);
                        break;
                    case LiteralInline literal:
                        stringBuilder.Append(literal.Content);
                        break;
                    case ContainerInline container:
                        ProcessInlineDelegate(container.FirstChild, stringBuilder);
                        break;
                    case HtmlEntityInline htmlEntity:
                        stringBuilder.Append(htmlEntity.Transcoded);
                        break;
                    case HtmlInline:
                        break;
                    default:
                        stringBuilder.Append(current);
                        break;
                }

                current = current.NextSibling;
            }
        }
    }
}

public class TocItem
{
    public int Level { get; set; }
    public required string Text { get; set; }
    public required string Id { get; set; }
}
