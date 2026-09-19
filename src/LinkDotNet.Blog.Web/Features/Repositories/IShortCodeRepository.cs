using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public interface IShortCodeRepository
{
    ValueTask<IPagedList<ShortCode>> GetAllAsync();

    ValueTask StoreAsync(ShortCode shortCode);

    ValueTask DeleteAsync(string id);
}
