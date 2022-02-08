using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services;

public interface IUserRecordService
{
    Task StoreUserRecordAsync();
}