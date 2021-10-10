using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.Web.Shared.Services.Sitemap
{
    public class SitemapService : ISitemapService
    {
        private readonly IRepository<BlogPost> repository;
        private readonly NavigationManager navigationManager;

        public SitemapService(
            IRepository<BlogPost> repository,
            NavigationManager navigationManager)
        {
            this.repository = repository;
            this.navigationManager = navigationManager;
        }

        public async Task<UrlSet> CreateSitemapAsync()
        {
            const string sitemapNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var urlSet = new UrlSet
            {
                Namespace = sitemapNamespace,
            };

            var blogPosts = (await repository.GetAllAsync(f => f.IsPublished, b => b.UpdatedDate)).ToList();

            urlSet.Urls.Add(new Url { Location = navigationManager.BaseUri });
            urlSet.Urls.AddRange(CreateUrlsForBlogPosts(blogPosts));
            urlSet.Urls.AddRange(CreateUrlsForTags(blogPosts));

            return urlSet;
        }

        private IEnumerable<Url> CreateUrlsForBlogPosts(IEnumerable<BlogPost> blogPosts)
        {
            return blogPosts.Select(b => new Url
            {
                Location = $"{navigationManager.BaseUri}blogPosts/{b.Id}",
                LastModified = b.UpdatedDate.ToString("yyyy-MM-dd"),
            }).ToList();
        }

        private IEnumerable<Url> CreateUrlsForTags(IEnumerable<BlogPost> blogPosts)
        {
            return blogPosts
                .SelectMany(b => b.Tags)
                .Select(t => t.Content)
                .Distinct()
                .Select(t => new Url
                {
                    Location = $"{navigationManager.BaseUri}searchByTag/{Uri.EscapeDataString(t)}",
                });
        }
    }
}