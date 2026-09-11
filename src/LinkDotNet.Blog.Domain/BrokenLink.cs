using System;

namespace LinkDotNet.Blog.Domain;

public sealed class BrokenLink : Entity
{
    public string BlogPostId { get; private set; } = default!;

    public string BlogPostTitle { get; private set; } = default!;

    public string Url { get; private set; } = default!;

    public string Reason { get; private set; } = default!;

    public DateTime CheckedDate { get; private set; }

    public static BrokenLink Create(string blogPostId, string blogPostTitle, string url, string reason, DateTime checkedDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blogPostId);
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return new BrokenLink
        {
            BlogPostId = blogPostId,
            BlogPostTitle = blogPostTitle,
            Url = url,
            Reason = reason,
            CheckedDate = checkedDate,
        };
    }
}
