using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Domain;

namespace LinkDotNet.Infrastructure.Persistence
{
    public interface IRepository
    {
        Task<BlogPost> GetByIdAsync(string blogPostId);

        Task<IEnumerable<BlogPost>> GetAllAsync(Expression<Func<BlogPost, bool>> filter = null, Expression<Func<BlogPost, object>> orderBy = null, bool descending = true);

        Task StoreAsync(BlogPost blogPost);

        Task DeleteAsync(string blogPostId);
    }
}