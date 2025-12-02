using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services;

public interface ICacheInvalidator
{
    Task ClearCacheAsync();
}
