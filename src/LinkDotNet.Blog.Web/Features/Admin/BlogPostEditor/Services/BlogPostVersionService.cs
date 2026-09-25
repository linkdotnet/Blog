using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

public sealed class BlogPostVersionService : IBlogPostVersionService
{
    private const int MaxAttempts = 3;

    private readonly IBlogPostRepository blogPostRepository;

    public BlogPostVersionService(IBlogPostRepository blogPostRepository)
    {
        this.blogPostRepository = blogPostRepository;
    }

    public ValueTask SaveNewVersionAsync(BlogPost currentBlogPost, BlogPost updatedBlogPost)
    {
        ArgumentNullException.ThrowIfNull(currentBlogPost);
        ArgumentNullException.ThrowIfNull(updatedBlogPost);

        return ReviseAsync(currentBlogPost.Id, (persisted, latestVersionNumber) => persisted.Revise(updatedBlogPost, latestVersionNumber));
    }

    public ValueTask<IReadOnlyList<BlogPostVersion>> GetVersionHistoryAsync(string blogPostId)
    {
        ArgumentException.ThrowIfNullOrEmpty(blogPostId);

        return blogPostRepository.GetVersionHistoryAsync(blogPostId);
    }

    public ValueTask RestoreVersionAsync(BlogPost currentBlogPost, BlogPostVersion targetVersion)
    {
        ArgumentNullException.ThrowIfNull(currentBlogPost);
        ArgumentNullException.ThrowIfNull(targetVersion);

        return ReviseAsync(currentBlogPost.Id, (persisted, latestVersionNumber) => persisted.RestoreFrom(targetVersion, latestVersionNumber));
    }

    private async ValueTask ReviseAsync(string blogPostId, Func<BlogPost, int, BlogPostVersion> revise)
    {
        for (var attempt = 1; attempt < MaxAttempts; attempt++)
        {
            try
            {
                await blogPostRepository.ReviseAsync(blogPostId, revise);
                return;
            }
            catch (BlogPostRevisionConflictException)
            {
                // Retrying is safe: the revision is re-applied to the freshly loaded post, so the competing change is snapshotted as well.
            }
        }

        await blogPostRepository.ReviseAsync(blogPostId, revise);
    }
}
