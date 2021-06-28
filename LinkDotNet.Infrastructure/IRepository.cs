using System.Collections.Generic;
using System.Threading.Tasks;
using LinkDotNet.Domain;

namespace LinkDotNet.Infrastructure
{
    public interface IRepository
    {
        Task<BlogPost> GetByIdAsync(string blogPostId);
        
        Task<IEnumerable<BlogPost>> GetAllAsync();
    }
}