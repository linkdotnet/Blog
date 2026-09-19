using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public interface IBlogPostTemplateRepository
{
    ValueTask<IPagedList<BlogPostTemplate>> GetAllAsync();

    ValueTask StoreAsync(BlogPostTemplate blogPostTemplate);

    ValueTask DeleteAsync(string id);
}
