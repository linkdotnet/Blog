using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BrokenLinks.Services;

public class LinkCheckerRegistrationTests
{
    [Fact]
    public async Task FailingLinkChecksShouldNotBeLoggedByHttpClient()
    {
        var loggerProvider = new RecordingLoggerProvider();
        var services = new ServiceCollection();
        services.AddLogging(b => b.AddProvider(loggerProvider).SetMinimumLevel(LogLevel.Trace));
        services.AddApplicationServices();
        services.AddHttpClient<ILinkChecker, LinkChecker>()
            .ConfigurePrimaryHttpMessageHandler(() => new FailingHandler());
        await using var provider = services.BuildServiceProvider();

        var result = await provider.GetRequiredService<ILinkChecker>()
            .CheckAsync(new Uri("https://broken.com"), TestContext.Current.CancellationToken);

        result.IsBroken.ShouldBeTrue();
        loggerProvider.LoggedCategories.ShouldNotContain(c => c.StartsWith("System.Net.Http.HttpClient", StringComparison.Ordinal));
    }

    private sealed class FailingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => throw new HttpRequestException("No such host is known.");
    }

    private sealed class RecordingLoggerProvider : ILoggerProvider
    {
        public ConcurrentBag<string> LoggedCategories { get; } = [];

        public ILogger CreateLogger(string categoryName) => new RecordingLogger(categoryName, LoggedCategories);

        public void Dispose()
        {
        }
    }

    private sealed class RecordingLogger(string categoryName, ConcurrentBag<string> loggedCategories) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => loggedCategories.Add(categoryName);
    }
}
