using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Shared.Services;

public interface IUserRecordService
{
    Task StoreUserRecordAsync();
}
