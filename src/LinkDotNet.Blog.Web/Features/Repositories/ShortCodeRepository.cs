using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public sealed class ShortCodeRepository(IRepository<ShortCode> repository) : IShortCodeRepository
{
    public ValueTask<IPagedList<ShortCode>> GetAllAsync() => repository.GetAllAsync();

    public ValueTask StoreAsync(ShortCode shortCode) => repository.StoreAsync(shortCode);

    public ValueTask DeleteAsync(string id) => repository.DeleteAsync(id);
}
