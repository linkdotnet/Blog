using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Home.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class ThemeTogglerTests : TestContext
{
    [Fact]
    public void ShouldSetSystemDefault()
    {
        Services.AddScoped(_ => Substitute.For<ILocalStorageService>());
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        JSInterop.Setup<string>("getCurrentSystemPreference").SetResult("dark");
        var setTheme = JSInterop.SetupVoid("setTheme", "dark");

        RenderComponent<ThemeToggler>();

        setTheme.Invocations.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldSetFromLocalStorage()
    {
        var localStorage = Substitute.For<ILocalStorageService>();
        localStorage.ContainKeyAsync("preferred-theme").Returns(true);
        localStorage.GetItemAsync<string>("preferred-theme").Returns("dark");
        Services.AddScoped(_ => localStorage);
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        JSInterop.Setup<string>("getCurrentSystemPreference").SetResult("light");
        var setTheme = JSInterop.SetupVoid("setTheme", "dark");

        RenderComponent<ThemeToggler>();

        setTheme.Invocations.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldSetValueAndSafeInStorage()
    {
        var localStorage = Substitute.For<ILocalStorageService>();
        Services.AddScoped(_ => localStorage);
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        JSInterop.Setup<string>("getCurrentSystemPreference").SetResult("light");
        JSInterop.SetupVoid("setTheme", "light");
        var setTheme = JSInterop.SetupVoid("setTheme", "dark")
            .SetVoidResult();
        var cut = RenderComponent<ThemeToggler>();

        cut.Find("span").Click();

        setTheme.Invocations.Should().NotBeNullOrEmpty();
        await localStorage.Received(1).SetItemAsync("preferred-theme", "dark");
    }
}
