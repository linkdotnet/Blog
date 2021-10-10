using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Shared.Services.Sitemap
{
    public interface ISitemapService
    {
        Task<SitemapUrlSet> CreateSitemapAsync();

        Task SaveSitemapToFileAsync(SitemapUrlSet sitemap);
    }
}