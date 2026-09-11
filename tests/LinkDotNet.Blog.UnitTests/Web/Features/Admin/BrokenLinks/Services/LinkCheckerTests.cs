using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BrokenLinks.Services;

public class LinkCheckerTests
{
    private static readonly Uri Url = new("https://link.com");

    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.TooManyRequests)]
    public async Task ShouldBeReachableWhenHeadSucceedsOrIsBlocked(HttpStatusCode statusCode)
    {
        var handler = new FakeHandler(_ => new HttpResponseMessage(statusCode));
        var sut = new LinkChecker(new HttpClient(handler));

        var result = await sut.CheckAsync(Url, CancellationToken.None);

        result.IsBroken.ShouldBeFalse();
        handler.Methods.ShouldBe([HttpMethod.Head]);
    }

    [Fact]
    public async Task ShouldFallBackToGetWhenHeadFails()
    {
        var handler = new FakeHandler(r => new HttpResponseMessage(
            r.Method == HttpMethod.Head ? HttpStatusCode.MethodNotAllowed : HttpStatusCode.OK));
        var sut = new LinkChecker(new HttpClient(handler));

        var result = await sut.CheckAsync(Url, CancellationToken.None);

        result.IsBroken.ShouldBeFalse();
        handler.Methods.ShouldBe([HttpMethod.Head, HttpMethod.Get]);
    }

    [Fact]
    public async Task ShouldBeBrokenWhenGetFailsAsWell()
    {
        var handler = new FakeHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var sut = new LinkChecker(new HttpClient(handler));

        var result = await sut.CheckAsync(Url, CancellationToken.None);

        result.IsBroken.ShouldBeTrue();
        result.Reason.ShouldBe("404 (NotFound)");
    }

    [Fact]
    public async Task ShouldBeBrokenWhenHostIsNotReachable()
    {
        var handler = new FakeHandler(_ => throw new HttpRequestException("No such host is known."));
        var sut = new LinkChecker(new HttpClient(handler));

        var result = await sut.CheckAsync(Url, CancellationToken.None);

        result.IsBroken.ShouldBeTrue();
        result.Reason.ShouldBe("No such host is known.");
    }

    [Fact]
    public async Task ShouldBeBrokenWhenRequestTimesOut()
    {
        var handler = new FakeHandler(_ => throw new TaskCanceledException());
        var sut = new LinkChecker(new HttpClient(handler));

        var result = await sut.CheckAsync(Url, CancellationToken.None);

        result.IsBroken.ShouldBeTrue();
        result.Reason.ShouldBe("Timeout");
    }

    [Fact]
    public async Task ShouldRethrowWhenCancelledByCaller()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var handler = new FakeHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var sut = new LinkChecker(new HttpClient(handler));

        await Should.ThrowAsync<OperationCanceledException>(() => sut.CheckAsync(Url, cts.Token));
    }

    private sealed class FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public List<HttpMethod> Methods { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Methods.Add(request.Method);
            return Task.FromResult(respond(request));
        }
    }
}
