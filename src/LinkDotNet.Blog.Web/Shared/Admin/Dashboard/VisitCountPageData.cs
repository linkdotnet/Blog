namespace LinkDotNet.Blog.Web.Shared.Admin.Dashboard;

public record VisitCountPageData
{
    public string Id { get; init; }

    public string Title { get; init; }

    public int Likes { get; init; }

    public int ClickCount { get; init; }
}
