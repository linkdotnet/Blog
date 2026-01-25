using Blazored.Toast.Services;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.Blog.Web.Features.Admin.Settings;
using LinkDotNet.Blog.Web.Features.MarkdownImport;
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
    public void GivenSettingsPage_WhenClicking_RunTransformerButton_JobIsTriggered()
    {
        var instantJobRegistry = Substitute.For<IInstantJobRegistry>();
        Services.AddScoped(_ => Substitute.For<ICacheInvalidator>());
        Services.AddScoped(_ => Options.Create<ApplicationConfiguration>(new ApplicationConfigurationBuilder().Build()));
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => instantJobRegistry);
        var cut = Render<SettingsPage>();
        var runTransformerButton = cut.Find("#run-visit-transformer");
        
        runTransformerButton.Click();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        instantJobRegistry.Received(1).RunInstantJob<TransformBlogPostRecordsJob>();
#pragma warning restore xUnit1051
    }

    [Fact]
    public void GivenMarkdownImportEnabled_WhenClicking_RunMarkdownImportButton_JobIsTriggered()
    {
        var instantJobRegistry = Substitute.For<IInstantJobRegistry>();
        var config = new ApplicationConfigurationBuilder()
            .WithMarkdownImport(true, "FlatDirectory", "https://example.com/markdown/")
            .Build();
        Services.AddScoped(_ => Substitute.For<ICacheInvalidator>());
        Services.AddScoped(_ => Options.Create(config));
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => instantJobRegistry);
        var cut = Render<SettingsPage>();
        var runImportButton = cut.Find("#run-markdown-import");
        
        runImportButton.Click();

#pragma warning disable xUnit1051 // Calls to methods which accept CancellationToken should use TestContext.Current.CancellationToken
        instantJobRegistry.Received(1).RunInstantJob<MarkdownImportJob>();
#pragma warning restore xUnit1051
    }

    [Fact]
    public void GivenMarkdownImportDisabled_MarkdownImportButtonNotShown()
    {
        var config = new ApplicationConfigurationBuilder()
            .WithMarkdownImport(false, "FlatDirectory", "")
            .Build();
        Services.AddScoped(_ => Substitute.For<ICacheInvalidator>());
        Services.AddScoped(_ => Options.Create(config));
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => Substitute.For<IInstantJobRegistry>());
        var cut = Render<SettingsPage>();
        
        var buttons = cut.FindAll("#run-markdown-import");

        buttons.ShouldBeEmpty();
    }
}
