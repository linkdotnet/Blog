using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

public sealed class SitemapService : ISitemapService
{
    private readonly IBlogPostListQuery blogPostListQuery;

    public SitemapService(IBlogPostListQuery blogPostListQuery)
    {
        this.blogPostListQuery = blogPostListQuery;
    }

    public async Task<SitemapUrlSet> CreateSitemapAsync(string baseUri)
    {
        ArgumentException.ThrowIfNullOrEmpty(baseUri);

        var urlSet = new SitemapUrlSet();

        if (!baseUri.EndsWith('/'))
        {
            baseUri += "/";
        }

        var blogPosts = await blogPostListQuery.GetPublishedAsync();

        urlSet.Urls.Add(new SitemapUrl { Location = baseUri });
        urlSet.Urls.Add(new SitemapUrl { Location = $"{baseUri}archive" });
        urlSet.Urls.AddRange(CreateUrlsForBlogPosts(blogPosts, baseUri));

        return urlSet;
    }

    private static ImmutableArray<SitemapUrl> CreateUrlsForBlogPosts(IEnumerable<BlogPostSummary> blogPosts, string baseUri)
    {
        return blogPosts.Select(b => new SitemapUrl
        {
            Location = baseUri + BlogPostRoute.For(b.Id, b.Slug),
            LastModified = b.UpdatedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        }).ToImmutableArray();
    }
}
