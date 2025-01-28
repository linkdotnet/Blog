using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

public sealed class SitemapService : ISitemapService
{
    private readonly IRepository<BlogPost> repository;

    public SitemapService(IRepository<BlogPost> repository)
    {
        this.repository = repository;
    }

    public async Task<SitemapUrlSet> CreateSitemapAsync(string baseUri)
    {
        ArgumentException.ThrowIfNullOrEmpty(baseUri);

        var urlSet = new SitemapUrlSet();

        if (!baseUri.EndsWith('/'))
        {
            baseUri += "/";
        }

        var blogPosts = await repository.GetAllAsync(f => f.IsPublished, b => b.UpdatedDate);

        urlSet.Urls.Add(new SitemapUrl { Location = baseUri });
        urlSet.Urls.Add(new SitemapUrl { Location = $"{baseUri}archive" });
        urlSet.Urls.AddRange(CreateUrlsForBlogPosts(blogPosts, baseUri));
        urlSet.Urls.AddRange(CreateUrlsForTags(blogPosts, baseUri));

        return urlSet;
    }

    private static ImmutableArray<SitemapUrl> CreateUrlsForBlogPosts(IEnumerable<BlogPost> blogPosts, string baseUri)
    {
        return blogPosts.Select(b => new SitemapUrl
        {
            Location = $"{baseUri}blogPost/{b.Id}",
            LastModified = b.UpdatedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        }).ToImmutableArray();
    }

    private static IEnumerable<SitemapUrl> CreateUrlsForTags(IEnumerable<BlogPost> blogPosts, string baseUri)
    {
        return blogPosts
            .SelectMany(b => b.Tags)
            .Distinct()
            .Select(t => new SitemapUrl
            {
                Location = $"{baseUri}searchByTag/{Uri.EscapeDataString(t)}",
            });
    }
}
