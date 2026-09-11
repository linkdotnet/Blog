namespace LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;

public sealed record LinkCheckResult(bool IsBroken, string Reason)
{
    public static LinkCheckResult Reachable { get; } = new(false, string.Empty);

    public static LinkCheckResult Broken(string reason) => new(true, reason);
}
