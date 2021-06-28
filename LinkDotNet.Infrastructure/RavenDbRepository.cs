using System.Net;
using System.Threading.Tasks;

namespace LinkDotNet.Infrastructure
{
    public class RavenDbRepository : IRepository
    {
        public Task GetAllBlogPostsAsync()
        {
            return Task.CompletedTask;
        }
    }
}