using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;

namespace LinkDotNet.Blog.Web.Controller;

[EnableRateLimiting("ip")]
[Route("sitemap.xml")]
public sealed class SitemapController : ControllerBase
{
    private readonly ISitemapService sitemapService;
    private readonly IXmlWriter xmlWriter;
    private readonly IMemoryCache memoryCache;

    public SitemapController(
        ISitemapService sitemapService,
        IXmlWriter xmlWriter,
        IMemoryCache memoryCache)
    {
        this.sitemapService = sitemapService;
        this.xmlWriter = xmlWriter;
        this.memoryCache = memoryCache;
    }

    [ResponseCache(Duration = 3600)]
    [HttpGet]
    public async Task<IActionResult> GetSitemap()
    {
        var buffer = await memoryCache.GetOrCreateAsync("sitemap.xml", async e =>
                     {
                         e.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                         return await GetSitemapBuffer();
                     })
                     ?? throw new InvalidOperationException("Buffer is null");

        return File(buffer, "application/xml");
    }

    private async Task<byte[]> GetSitemapBuffer()
    {
        var baseUri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var sitemap = await sitemapService.CreateSitemapAsync(baseUri);
        var buffer = await xmlWriter.WriteToBuffer(sitemap);
        return buffer;
    }
}
