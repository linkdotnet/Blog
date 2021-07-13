using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Domain;
using X.PagedList;

namespace LinkDotNet.Infrastructure.Persistence
{
    public interface IRepository
    {
        Task<BlogPost> GetByIdAsync(string blogPostId);

        Task<IPagedList<BlogPost>> GetAllAsync(
            Expression<Func<BlogPost, bool>> filter = null,
            Expression<Func<BlogPost,
            object>> orderBy = null,
            bool descending = true,
            int page = 1,
            int pageSize = 5);

        Task StoreAsync(BlogPost blogPost);

        Task DeleteAsync(string blogPostId);
    }
}