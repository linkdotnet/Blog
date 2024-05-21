using Blazored.Toast.Services;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Admin.Settings;
using LinkDotNet.Blog.Web.Features.Services;
using NCronJob;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Settings;

public class SettingsPageTests : BunitContext
{
    [Fact]
    public void GivenSettingsPage_WhenClicking_InvalidateCacheButton_TokenIsCancelled()
    {
        var cacheInvalidator = Substitute.For<ICacheInvalidator>();
        Services.AddScoped(_ => cacheInvalidator);
        Services.AddScoped(_ => Options.Create<ApplicationConfiguration>(new()));
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => Substitute.For<IInstantJobRegistry>());
        var cut = Render<SettingsPage>();
        var invalidateCacheButton = cut.Find("#invalidate-cache");
        
        invalidateCacheButton.Click();

        cacheInvalidator.Received(1).Cancel();
    }
}
