﻿using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

public interface ISitemapService
{
    Task<SitemapUrlSet> CreateSitemapAsync(string baseUri);
}
