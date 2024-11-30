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
        .Build();

    public static MarkupString ToMarkupString(string markdown)
    {
        return string.IsNullOrEmpty(markdown)
            ? default
            : (MarkupString)Markdown.ToHtml(markdown, MarkdownPipeline);
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
