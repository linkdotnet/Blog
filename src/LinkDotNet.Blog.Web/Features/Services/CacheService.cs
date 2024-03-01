using System;
using System.Threading;

namespace LinkDotNet.Blog.Web.Features.Services;

public sealed class CacheService : ICacheTokenProvider, ICacheInvalidator, IDisposable
{
    private CancellationTokenSource cancellationTokenSource = new();

    public CancellationToken Token => cancellationTokenSource.Token;

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new();
    }

    public void Dispose() => cancellationTokenSource.Dispose();
}
