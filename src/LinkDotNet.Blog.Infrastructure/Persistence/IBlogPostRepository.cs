using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public interface IBlogPostRepository
{
    ValueTask<BlogPost?> GetAsync(string blogPostId);

    /// <summary>
    /// Loads the persisted blog post and its latest version number, lets <paramref name="revise"/> change the post
    /// and create the snapshot, then stores both as one unit of work.
    /// </summary>
    /// <exception cref="BlogPostRevisionConflictException">The post or its version number was changed concurrently.</exception>
    ValueTask ReviseAsync(string blogPostId, Func<BlogPost, int, BlogPostVersion> revise);

    /// <summary>
    /// Returns all versions of the blog post, newest first.
    /// </summary>
    ValueTask<IReadOnlyList<BlogPostVersion>> GetVersionHistoryAsync(string blogPostId);

    ValueTask LikeAsync(string blogPostId);

    ValueTask UnlikeAsync(string blogPostId);

    /// <summary>
    /// Deletes the blog post together with its versions.
    /// </summary>
    ValueTask DeleteAsync(string blogPostId);
}
