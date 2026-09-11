using System;
using System.Threading;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;

public interface ILinkChecker
{
    Task<LinkCheckResult> CheckAsync(Uri url, CancellationToken token);
}
