using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Shared.Services.Sitemap
{
    public interface ISitemapService
    {
        Task<UrlSet> CreateSitemapAsync();
    }
}