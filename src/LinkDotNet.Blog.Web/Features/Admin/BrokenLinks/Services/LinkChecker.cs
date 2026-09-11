using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;

public sealed class LinkChecker : ILinkChecker
{
    private readonly HttpClient httpClient;

    public LinkChecker(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<LinkCheckResult> CheckAsync(Uri url, CancellationToken token)
    {
        try
        {
            using var headResponse = await SendAsync(HttpMethod.Head, url, token);
            if (IsReachable(headResponse.StatusCode))
            {
                return LinkCheckResult.Reachable;
            }

            // Plenty of servers reject or mishandle HEAD, so only a failing GET counts as broken
            using var getResponse = await SendAsync(HttpMethod.Get, url, token);
            return IsReachable(getResponse.StatusCode)
                ? LinkCheckResult.Reachable
                : LinkCheckResult.Broken($"{(int)getResponse.StatusCode} ({getResponse.StatusCode})");
        }
        catch (HttpRequestException e)
        {
            return LinkCheckResult.Broken(e.Message);
        }
        catch (TaskCanceledException) when (!token.IsCancellationRequested)
        {
            return LinkCheckResult.Broken("Timeout");
        }
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, Uri url, CancellationToken token)
    {
        using var request = new HttpRequestMessage(method, url);
        return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
    }

    // 401, 403 and 429 mostly come from auth walls or bot protection, the page itself exists
    private static bool IsReachable(HttpStatusCode statusCode) =>
        (int)statusCode < 400 || statusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests;
}
