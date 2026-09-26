using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NCronJob;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.Features.Services.Similiarity;

namespace LinkDotNet.Blog.Web.Features;

public class SimilarBlogPostJob : IJob
{
    private readonly IRepository<BlogPost> blogPostRepository;
    private readonly IRepository<SimilarBlogPost> similarBlogPostRepository;
    private readonly ICacheInvalidator cacheInvalidator;

    public SimilarBlogPostJob(
        IRepository<BlogPost> blogPostRepository,
        IRepository<SimilarBlogPost> similarBlogPostRepository,
        ICacheInvalidator cacheInvalidator)
    {
        this.blogPostRepository = blogPostRepository;
        this.similarBlogPostRepository = similarBlogPostRepository;
        this.cacheInvalidator = cacheInvalidator;
    }

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);

        var isInstantJobTriggered = context.Parameter is not null;
        var noJobPublished = context.ParentOutput is null or 0;
        if (noJobPublished && !isInstantJobTriggered)
        {
            return;
        }

        var blogPosts = await blogPostRepository.GetAllByProjectionAsync(
            bp => new BlogPostSimilarity { Id = bp.Id, Title = bp.Title, Tags = bp.Tags, ShortDescription = bp.ShortDescription },
            f => f.IsPublished);
        var documents = blogPosts.Select(bp => TextProcessor.TokenizeAndNormalize([bp.Title, bp.ShortDescription, ..bp.Tags])).ToList();

        var similarities = blogPosts.Select(bp => GetSimilarityForBlogPost(bp, documents, blogPosts)).ToArray();
        var ids = await similarBlogPostRepository.GetAllByProjectionAsync(s => s.Id);
        try
        {
            await similarBlogPostRepository.DeleteBulkAsync(ids);
            await similarBlogPostRepository.StoreBulkAsync(similarities);
        }
        finally
        {
            await cacheInvalidator.ClearBlogPostPagesAsync();
        }
    }

    private static SimilarBlogPost GetSimilarityForBlogPost(
        BlogPostSimilarity blogPost,
        List<IReadOnlyCollection<string>> documents,
        IReadOnlyCollection<BlogPostSimilarity> blogPosts)
    {
        var target = TextProcessor.TokenizeAndNormalize([blogPost.Title, blogPost.ShortDescription, ..blogPost.Tags]);

        var vectorizer = new TfIdfVectorizer(documents);
        var targetVector = vectorizer.ComputeTfIdfVector(target);

        var similarBlogPosts = blogPosts
            .Select((bp, index) => new
            {
                BlogPost = bp,
                Similarity = SimilarityCalculator.CosineSimilarity(targetVector, vectorizer.ComputeTfIdfVector(documents[index]))
            })
            .Where(s => s.BlogPost.Id != blogPost.Id)
            .OrderByDescending(x => x.Similarity)
            .Take(3)
            .Select(s => s.BlogPost.Id)
            .ToArray();

        return new SimilarBlogPost { Id = SimilarBlogPost.IdFor(blogPost.Id), SimilarBlogPostIds = similarBlogPosts };
    }

    // Member-init on purpose: RavenDB can't project into constructors with parameters.
    private sealed record BlogPostSimilarity
    {
        public required string Id { get; init; }

        public required string Title { get; init; }

        public required IList<string> Tags { get; init; }

        public required string ShortDescription { get; init; }
    }
}
