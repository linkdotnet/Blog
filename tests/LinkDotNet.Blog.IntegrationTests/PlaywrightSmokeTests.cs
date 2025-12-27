using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.TestUtilities;
using Microsoft.Playwright;

namespace LinkDotNet.Blog.IntegrationTests;

[Collection("Sequential")]
public sealed class PlaywrightSmokeTests : IClassFixture<PlaywrightWebApplicationFactory>, IAsyncDisposable
{
    private readonly PlaywrightWebApplicationFactory factory;
    private readonly Lazy<Task<IPlaywright>> playwrightTask;
    private readonly Lazy<Task<IBrowser>> browserTask;

    public PlaywrightSmokeTests(PlaywrightWebApplicationFactory factory)
    {
        this.factory = factory;
        _ = factory.CreateClient(); // Initialize the factory
        playwrightTask = new Lazy<Task<IPlaywright>>(() => Playwright.CreateAsync());
        browserTask = new Lazy<Task<IBrowser>>(async () => 
        {
            var playwright = await playwrightTask.Value;
            return await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        });
    }

    [Fact]
    public async Task ShouldNavigateToHomePageAndShowBlogPosts()
    {
        var browser = await browserTask.Value;
        var page = await browser.NewPageAsync();

        await page.GotoAsync(factory.ServerAddress, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        var blogPostElements = await page.QuerySelectorAllAsync("article");
        blogPostElements.Count.ShouldBeGreaterThan(0);

        var title = await page.TitleAsync();
        title.ShouldBe("Integration Tests Blog");
    }

    [Fact]
    public async Task ShouldNavigateToBlogPostAndShowContent()
    {
        var browser = await browserTask.Value;
        var page = await browser.NewPageAsync();

        await page.GotoAsync(factory.ServerAddress, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        // Find and click the first blog post link
        var blogPostLink = await page.QuerySelectorAsync("article a[href*='/blogPost/']");
        blogPostLink.ShouldNotBeNull();

        var href = await blogPostLink.GetAttributeAsync("href");
        href.ShouldNotBeNull();

        await page.GotoAsync($"{factory.ServerAddress.TrimEnd('/')}{href}", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        // Verify we're on a blog post page
        var heading = await page.QuerySelectorAsync("h1");
        heading.ShouldNotBeNull();
        var headingText = await heading.TextContentAsync();
        headingText.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ShouldNavigateToSecondPageAndShowBlogPosts()
    {
        var browser = await browserTask.Value;
        var page = await browser.NewPageAsync();

        await page.GotoAsync(factory.ServerAddress, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        // Look for pagination button/link for page 2
        var page2Link = await page.QuerySelectorAsync("a[href='/2']");
        if (page2Link is not null)
        {
            await page2Link.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Verify we're on page 2
            var currentUrl = page.Url;
            currentUrl.ShouldContain("/2");

            // Verify blog posts are shown on page 2
            var blogPostElements = await page.QuerySelectorAllAsync("article");
            blogPostElements.Count.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    public async Task ShouldClickOnBlogPostLinkAndNavigate()
    {
        var browser = await browserTask.Value;
        var page = await browser.NewPageAsync();

        await page.GotoAsync(factory.ServerAddress, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

        // Find the first blog post link
        var blogPostLink = await page.QuerySelectorAsync("article a[href*='/blogPost/']");
        blogPostLink.ShouldNotBeNull();

        // Get the expected URL
        var expectedHref = await blogPostLink.GetAttributeAsync("href");
        expectedHref.ShouldNotBeNull();

        // Click the link
        await blogPostLink.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verify navigation occurred
        var currentUrl = page.Url;
        currentUrl.ShouldContain("/blogPost/");
    }

    public async ValueTask DisposeAsync()
    {
        if (browserTask.IsValueCreated)
        {
            var browser = await browserTask.Value;
            await browser.DisposeAsync();
        }
        
        if (playwrightTask.IsValueCreated)
        {
            var playwright = await playwrightTask.Value;
            playwright.Dispose();
        }
    }
}
