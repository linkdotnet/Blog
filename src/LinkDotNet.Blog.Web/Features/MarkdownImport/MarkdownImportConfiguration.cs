namespace LinkDotNet.Blog.Web.Features.MarkdownImport;

public sealed record MarkdownImportConfiguration
{
    public bool Enabled { get; init; }

    public string SourceType { get; init; } = "FlatDirectory";

    public string Url { get; init; } = string.Empty;
}
