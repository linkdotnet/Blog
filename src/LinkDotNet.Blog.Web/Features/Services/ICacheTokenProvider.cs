using System.Threading;

namespace LinkDotNet.Blog.Web.Features.Services;

public interface ICacheTokenProvider
{
    CancellationToken Token { get; }
}
