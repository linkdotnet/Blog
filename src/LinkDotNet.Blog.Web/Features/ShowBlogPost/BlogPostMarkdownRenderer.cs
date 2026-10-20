using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.ShowBlogPost;

public static class BlogPostMarkdownRenderer
{
    public static string Render(BlogPost blogPost, IEnumerable<ShortCode> shortCodes, string canonicalUrl, bool includeAuthor)
    {
        ArgumentNullException.ThrowIfNull(blogPost);

        var sb = new StringBuilder();
        sb.Append("---\n");
        AppendScalar(sb, "title", blogPost.Title);
        AppendScalar(sb, "description", blogPost.ShortDescription);
        if (includeAuthor && blogPost.AuthorName is not null)
        {
            AppendScalar(sb, "author", blogPost.AuthorName);
        }

        sb.Append(CultureInfo.InvariantCulture, $"updated: {blogPost.UpdatedDate:yyyy-MM-dd}\n");
        if (blogPost.Tags is { Count: > 0 })
        {
            sb.Append("tags: [").AppendJoin(", ", blogPost.Tags.Select(ToYamlString)).Append("]\n");
        }

        AppendScalar(sb, "canonical", canonicalUrl);
        sb.Append("---\n\n");
        sb.Append("# ").Append(blogPost.Title).Append("\n\n");
        sb.Append(MarkdownConverter.ExpandShortCodes(blogPost.Content, shortCodes));
        return sb.ToString();
    }

    private static void AppendScalar(StringBuilder sb, string key, string value)
        => sb.Append(key).Append(": ").Append(ToYamlString(value)).Append('\n');

    private static string ToYamlString(string value)
    {
        var escaped = value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
        return $"\"{escaped}\"";
    }
}
