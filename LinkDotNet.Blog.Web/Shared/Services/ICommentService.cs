using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Shared.Services
{
    public interface ICommentService
    {
        Task EnableCommentSection(string className);
    }
}