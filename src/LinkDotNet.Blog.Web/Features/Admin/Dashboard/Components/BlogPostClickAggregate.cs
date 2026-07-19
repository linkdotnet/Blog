namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;

public readonly record struct BlogPostClickAggregate
{
    public string BlogPostId { get; init; }

    public int ClickCount { get; init; }
}
