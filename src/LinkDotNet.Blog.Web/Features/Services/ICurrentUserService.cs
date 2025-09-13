using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services;

public interface ICurrentUserService
{
    ValueTask<string?> GetDisplayNameAsync();
}
