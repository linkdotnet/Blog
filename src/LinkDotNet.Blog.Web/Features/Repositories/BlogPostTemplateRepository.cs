using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;

namespace LinkDotNet.Blog.Web.Features.Repositories;

public sealed class BlogPostTemplateRepository(IRepository<BlogPostTemplate> repository) : IBlogPostTemplateRepository
{
    public ValueTask<IPagedList<BlogPostTemplate>> GetAllAsync() => repository.GetAllAsync();

    public ValueTask StoreAsync(BlogPostTemplate blogPostTemplate) => repository.StoreAsync(blogPostTemplate);

    public ValueTask DeleteAsync(string id) => repository.DeleteAsync(id);
}
