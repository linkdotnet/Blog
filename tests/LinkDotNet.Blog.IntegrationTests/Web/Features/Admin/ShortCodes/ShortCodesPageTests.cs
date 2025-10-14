using System.Linq;
using System.Threading.Tasks;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities.Fakes;
using LinkDotNet.Blog.Web.Features.Admin.ShortCodes;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.ShortCodes;

public sealed class ShortCodesPageTests : SqlDatabaseTestBase<ShortCode>
{
    [Fact]
    public async Task ShouldShowShortCodes()
    {
        await using var ctx = new BunitContext();
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        var cut = ctx.Render<ShortCodesPage>();
        cut.Find("#short-code-content").Input("# Text");
        cut.Find("#short-code-name").Change("ShortName");
        
        await cut.Find("form").SubmitAsync();
        
        var shortCodes = await Repository.GetAllAsync();
        shortCodes.ShouldHaveSingleItem();
        shortCodes.First().MarkdownContent.ShouldBe("# Text");
        shortCodes.First().Name.ShouldBe("ShortName");
    }
    
    [Fact]
    public async Task ShouldUpdateShortCode()
    {
        await using var ctx = new BunitContext();
        var shortCode = ShortCode.Create("# Text", "ShortName");
        await Repository.StoreAsync(shortCode);
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        ctx.ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        var cut = ctx.Render<ShortCodesPage>();
        await cut.Find("#edit-shortcode").ClickAsync();
        cut.Find("#short-code-content").Input("# New Text");
        cut.Find("#short-code-name").Change("ShortName");
        
        await cut.Find("form").SubmitAsync();
        
        var shortCodes = await Repository.GetAllAsync();
        shortCodes.ShouldHaveSingleItem();
        shortCodes.First().MarkdownContent.ShouldBe("# New Text");
        shortCodes.First().Name.ShouldBe("ShortName");
    }
}
