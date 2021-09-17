using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Shared.Services
{
    public interface IGiscusService
    {
        Task EnableCommentSection(string className);
    }
}