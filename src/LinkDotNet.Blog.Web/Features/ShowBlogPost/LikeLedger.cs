using System;
using System.Collections.Generic;

namespace LinkDotNet.Blog.Web.Features.ShowBlogPost;

/// <summary>
/// Scoped per circuit. The client-side "has liked" flag can be cleared at will,
/// so the server caps every circuit at one net like or unlike per post.
/// </summary>
public sealed class LikeLedger
{
    private readonly Dictionary<string, int> netLikesPerBlogPost = [];

    public bool TryRecord(string blogPostId, bool hasLiked)
    {
        var netLikes = netLikesPerBlogPost.GetValueOrDefault(blogPostId) + (hasLiked ? 1 : -1);
        if (Math.Abs(netLikes) > 1)
        {
            return false;
        }

        netLikesPerBlogPost[blogPostId] = netLikes;
        return true;
    }
}
