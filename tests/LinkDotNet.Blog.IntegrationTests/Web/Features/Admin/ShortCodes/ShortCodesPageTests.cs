using System;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.TestUtilities.Fakes;
using LinkDotNet.Blog.Web.Features.Admin.ShortCodes;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Locking.AsyncKeyed;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.ShortCodes;

public sealed class ShortCodesPageTests : SqlDatabaseTestBase<ShortCode>, IDisposable
{
    private readonly BunitContext ctx = new();
    private readonly IRepository<BlogPost> blogPostRepository = Substitute.For<IRepository<BlogPost>>();

    public ShortCodesPageTests()
    {
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => blogPostRepository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        ctx.Services.AddScoped<IFusionCache>(_ => new FusionCache(new FusionCacheOptions(), memoryLocker: new AsyncKeyedMemoryLocker()));
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        ctx.JSInterop.SetupNativeDialog();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        SetupPostContents();
    }

    [Fact]
    public async Task ShouldCreateShortCode()
    {
        var cut = ctx.Render<ShortCodesPage>();
        cut.Find(".new-button").Click();
        cut.Find("#short-code-name").Input("ShortName");
        cut.Find("#short-code-content").Input("# Text");

        await cut.Find("form").SubmitAsync();

        var shortCodes = await Repository.GetAllAsync();
        shortCodes.ShouldHaveSingleItem();
        shortCodes.First().MarkdownContent.ShouldBe("# Text");
        shortCodes.First().Name.ShouldBe("ShortName");
    }

    [Fact]
    public async Task ShouldUpdateShortCode()
    {
        await Repository.StoreAsync(ShortCode.Create("ShortName", "# Text"));
        var cut = ctx.Render<ShortCodesPage>();
        cut.Find(".card-action:not(.destructive)").Click();
        cut.Find("#short-code-content").Input("# New Text");

        await cut.Find("form").SubmitAsync();

        var shortCodes = await Repository.GetAllAsync();
        shortCodes.ShouldHaveSingleItem();
        shortCodes.First().MarkdownContent.ShouldBe("# New Text");
        shortCodes.First().Name.ShouldBe("ShortName");
    }

    [Fact]
    public async Task ShouldDeleteShortCodeAfterConfirmation()
    {
        await Repository.StoreAsync(ShortCode.Create("ShortName", "# Text"));
        var cut = ctx.Render<ShortCodesPage>();

        cut.Find(".card-action.destructive").Click();
        await cut.Find(".alert-button.destructive").ClickAsync();

        (await Repository.GetAllAsync()).ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldKeepShortCodeWhenDeletionIsCancelled()
    {
        await Repository.StoreAsync(ShortCode.Create("ShortName", "# Text"));
        var cut = ctx.Render<ShortCodesPage>();

        cut.Find(".card-action.destructive").Click();
        cut.Find(".alert-button:not(.destructive)").Click();

        (await Repository.GetAllAsync()).ShouldHaveSingleItem();
    }

    [Fact]
    public async Task ShouldNotSaveShortCodeWithDuplicateName()
    {
        await Repository.StoreAsync(ShortCode.Create("ShortName", "# Text"));
        var cut = ctx.Render<ShortCodesPage>();
        cut.Find(".new-button").Click();
        cut.Find("#short-code-name").Input("shortname");
        cut.Find("#short-code-content").Input("# Other");

        await cut.Find("form").SubmitAsync();

        (await Repository.GetAllAsync()).ShouldHaveSingleItem();
        cut.Find("#name-hint").ClassList.ShouldContain("error");
    }

    [Fact]
    public async Task ShouldShowUsageCount()
    {
        await Repository.StoreAsync(ShortCode.Create("promo", "Buy now"));
        SetupPostContents("Intro [[promo]]", "Other [[promo]]", "Nothing here");

        var cut = ctx.Render<ShortCodesPage>();

        cut.Markup.ShouldContain("Used in 2 posts");
    }

    [Fact]
    public void ShouldOfferTemplatesWhenThereAreNoShortCodes()
    {
        var cut = ctx.Render<ShortCodesPage>();

        cut.FindAll(".template-tile").Count.ShouldBe(ShortCodeTemplate.All.Count);
    }

    [Fact]
    public async Task ShouldPrefillEditorFromTemplate()
    {
        var cut = ctx.Render<ShortCodesPage>();
        var template = ShortCodeTemplate.All[0];

        cut.Find(".template-tile").Click();
        cut.Find("#short-code-name").Input(template.SuggestedName);
        await cut.Find("form").SubmitAsync();

        var shortCode = (await Repository.GetAllAsync()).ShouldHaveSingleItem();
        shortCode.MarkdownContent.ShouldBe(template.MarkdownContent);
    }

    public void Dispose() => ctx.Dispose();

    private void SetupPostContents(params string[] contents)
    {
        blogPostRepository.GetAllByProjectionAsync(Arg.Any<System.Linq.Expressions.Expression<Func<BlogPost, string>>>())
            .ReturnsForAnyArgs(new PagedList<string>(contents, contents.Length, 1, Math.Max(contents.Length, 1)));
    }
}
