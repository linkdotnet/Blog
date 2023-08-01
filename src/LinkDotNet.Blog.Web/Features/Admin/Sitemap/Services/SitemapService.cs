using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

public sealed class SitemapService : ISitemapService
{
    private readonly IRepository<BlogPost> repository;
    private readonly NavigationManager navigationManager;
    private readonly IXmlFileWriter xmlFileWriter;

    public SitemapService(
        IRepository<BlogPost> repository,
        NavigationManager navigationManager,
        IXmlFileWriter xmlFileWriter)
    {
        this.repository = repository;
        this.navigationManager = navigationManager;
        this.xmlFileWriter = xmlFileWriter;
    }

    public async Task<SitemapUrlSet> CreateSitemapAsync()
    {
        var urlSet = new SitemapUrlSet();

        var blogPosts = await repository.GetAllAsync(f => f.IsPublished, b => b.UpdatedDate);

        urlSet.Urls.Add(new SitemapUrl { Location = navigationManager.BaseUri });
        urlSet.Urls.Add(new SitemapUrl { Location = $"{navigationManager.BaseUri}archive" });
        urlSet.Urls.AddRange(CreateUrlsForBlogPosts(blogPosts));
        urlSet.Urls.AddRange(CreateUrlsForTags(blogPosts));

        return urlSet;
    }

    public async Task SaveSitemapToFileAsync(SitemapUrlSet sitemap)
    {
        await xmlFileWriter.WriteObjectToXmlFileAsync(sitemap, "wwwroot/sitemap.xml");
    }

    private IEnumerable<SitemapUrl> CreateUrlsForBlogPosts(IEnumerable<BlogPost> blogPosts)
    {
        return blogPosts.Select(b => new SitemapUrl
        {
            Location = $"{navigationManager.BaseUri}blogPost/{b.Id}",
            LastModified = b.UpdatedDate.ToString("yyyy-MM-dd"),
        }).ToImmutableArray();
    }

    private IEnumerable<SitemapUrl> CreateUrlsForTags(IEnumerable<BlogPost> blogPosts)
    {
        return blogPosts
            .SelectMany(b => b.Tags)
            .Distinct()
            .Select(t => new SitemapUrl
            {
                Location = $"{navigationManager.BaseUri}searchByTag/{Uri.EscapeDataString(t)}",
            });
    }
}