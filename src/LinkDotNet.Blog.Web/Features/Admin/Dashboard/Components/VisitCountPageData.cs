namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;

public readonly record struct VisitCountPageData
{
    public string Id { get; init; }

    public string Title { get; init; }

    public int Likes { get; init; }

    public int ClickCount { get; init; }
}
