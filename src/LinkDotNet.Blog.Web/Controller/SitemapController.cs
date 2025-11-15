using LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

namespace LinkDotNet.Blog.Web.Controller;

[EnableRateLimiting("ip")]
[Route("sitemap.xml")]
public sealed class SitemapController : ControllerBase
{
    private readonly ISitemapService sitemapService;
    private readonly IXmlWriter xmlWriter;
    private readonly IFusionCache fusionCache;

    public SitemapController(
        ISitemapService sitemapService,
        IXmlWriter xmlWriter,
        IFusionCache fusionCache)
    {
        this.sitemapService = sitemapService;
        this.xmlWriter = xmlWriter;
        this.fusionCache = fusionCache;
    }

    [ResponseCache(Duration = 3600)]
    [HttpGet]
    public async Task<IActionResult> GetSitemap()
    {
        var buffer = await fusionCache.GetOrSetAsync("sitemap.xml", async e => await GetSitemapBuffer(), o => o.SetDuration(TimeSpan.FromHours(1)))
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
