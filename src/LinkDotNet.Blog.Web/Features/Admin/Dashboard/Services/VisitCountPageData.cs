namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;

public sealed record VisitCountPageData
{
    public string Id { get; init; }

    public string Title { get; init; }

    public int Likes { get; init; }

    public int ClickCount { get; init; }
}