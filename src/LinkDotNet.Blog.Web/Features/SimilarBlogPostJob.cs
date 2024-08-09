using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NCronJob;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Services.Similiarity;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.Features;

public class SimilarBlogPostJob : IJob
{
    private readonly IRepository<BlogPost> blogPostRepository;
    private readonly IRepository<SimilarBlogPost> similarBlogPostRepository;
    private readonly bool showSimilarPosts;

    public SimilarBlogPostJob(
        IRepository<BlogPost> blogPostRepository,
        IRepository<SimilarBlogPost> similarBlogPostRepository,
        IOptions<ApplicationConfiguration> applicationConfiguration)
    {
        ArgumentNullException.ThrowIfNull(applicationConfiguration);

        this.blogPostRepository = blogPostRepository;
        this.similarBlogPostRepository = similarBlogPostRepository;
        showSimilarPosts = applicationConfiguration.Value.ShowSimilarPosts;
    }

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!showSimilarPosts)
        {
            return;
        }

        var isInstantJobTriggered = context.Parameter is not null;
        var noJobPublished = context.ParentOutput is null or 0;
        if (noJobPublished && !isInstantJobTriggered)
        {
            return;
        }

        var blogPosts = await blogPostRepository.GetAllByProjectionAsync(
            bp => new BlogPostSimilarity(bp.Id, bp.Title, bp.Tags, bp.ShortDescription),
            f => f.IsPublished);
        var documents = blogPosts.Select(bp => TextProcessor.TokenizeAndNormalize([bp.Title, bp.ShortDescription, ..bp.Tags])).ToList();

        var similarities = blogPosts.Select(bp => GetSimilarityForBlogPost(bp, documents, blogPosts)).ToArray();
        var ids = await similarBlogPostRepository.GetAllByProjectionAsync(s => s.Id);
        await similarBlogPostRepository.DeleteBulkAsync(ids);
        await similarBlogPostRepository.StoreBulkAsync(similarities);

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

        return new SimilarBlogPost { Id = blogPost.Id, SimilarBlogPostIds = similarBlogPosts };
    }

    private sealed record BlogPostSimilarity(string Id, string Title, IList<string> Tags, string ShortDescription);
}
