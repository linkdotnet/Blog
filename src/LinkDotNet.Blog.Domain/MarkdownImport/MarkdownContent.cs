namespace LinkDotNet.Blog.Domain.MarkdownImport;

public sealed record MarkdownContent(
    MarkdownMetadata Metadata,
    string ShortDescription,
    string Content);
