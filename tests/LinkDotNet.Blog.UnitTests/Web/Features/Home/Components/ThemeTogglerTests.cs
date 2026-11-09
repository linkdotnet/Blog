using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class ThemeTogglerTests : BunitContext
{
    [Fact]
    public void ShouldRenderBothIconsSoCssCanPickOneBeforeInteractivity()
    {
        var cut = Render<ThemeToggler>();

        cut.FindAll(".theme-icon-light").Count.ShouldBe(1);
        cut.FindAll(".theme-icon-dark").Count.ShouldBe(1);
        JSInterop.Invocations.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldToggleThemeOnClick()
    {
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        var toggleTheme = JSInterop.SetupVoid("toggleTheme").SetVoidResult();
        var cut = Render<ThemeToggler>();

        await cut.Find("div").ClickAsync();

        toggleTheme.Invocations.Count.ShouldBe(1);
    }
}
