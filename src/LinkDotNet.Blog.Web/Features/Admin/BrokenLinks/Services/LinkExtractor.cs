using System;
using System.Collections.Generic;
using System.Linq;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;

public static class LinkExtractor
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAutoLinks()
        .Build();

    public static IReadOnlyCollection<Uri> ExtractAbsoluteUrls(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return [];
        }

        var document = Markdown.Parse(markdown, Pipeline);

        var linkUrls = document.Descendants<LinkInline>().Select(l => l.Url);
        var autoLinkUrls = document.Descendants<AutolinkInline>().Where(a => !a.IsEmail).Select(a => a.Url);

        return linkUrls
            .Concat(autoLinkUrls)
            .Select(ToHttpUri)
            .OfType<Uri>()
            .Distinct()
            .ToArray();
    }

    private static Uri? ToHttpUri(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            ? uri
            : null;
    }
}
