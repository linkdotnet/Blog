using System.Threading;
using Blazored.Toast.Services;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Admin.BrokenLinks.Services;
using LinkDotNet.Blog.Web.Features.Admin.Settings;
using LinkDotNet.Blog.Web.Features.Services;
using NCronJob;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Settings;

public class SettingsPageTests : BunitContext
{
    [Fact]
    public void GivenSettingsPage_WhenClicking_InvalidateCacheButton_CacheIsCleared()
    {
        var cacheInvalidator = Substitute.For<ICacheInvalidator>();
        Services.AddScoped(_ => cacheInvalidator);
        Services.AddScoped(_ => Options.Create<ApplicationConfiguration>(new ApplicationConfigurationBuilder().Build()));
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => Substitute.For<IInstantJobRegistry>());
        var cut = Render<SettingsPage>();
        var invalidateCacheButton = cut.Find("#invalidate-cache");
        
        invalidateCacheButton.Click();

        cacheInvalidator.Received(1).ClearCacheAsync();
    }

    [Fact]
    public void GivenSettingsPage_WhenClicking_RunBrokenLinkChecker_JobIsStarted()
    {
        var instantJobRegistry = RegisterServices(enableBrokenLinkChecker: true);
        var cut = Render<SettingsPage>();

        cut.Find("#run-broken-link-checker").Click();

        instantJobRegistry.Received(1).RunInstantJob<BrokenLinkCheckerJob>(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void GivenBrokenLinkCheckerIsDisabled_RunButtonIsDisabled()
    {
        RegisterServices(enableBrokenLinkChecker: false);

        var cut = Render<SettingsPage>();

        cut.Find("#run-broken-link-checker").HasAttribute("disabled").ShouldBeTrue();
    }

    private IInstantJobRegistry RegisterServices(bool enableBrokenLinkChecker)
    {
        var instantJobRegistry = Substitute.For<IInstantJobRegistry>();
        Services.AddScoped(_ => Substitute.For<ICacheInvalidator>());
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().WithEnableBrokenLinkChecker(enableBrokenLinkChecker).Build()));
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => instantJobRegistry);
        return instantJobRegistry;
    }
}
