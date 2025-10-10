using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Home.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class ThemeTogglerTests : BunitContext
{
    [Fact]
    public void ShouldSetSystemDefault()
    {
        Services.AddScoped(_ => Substitute.For<ILocalStorageService>());
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        JSInterop.Setup<string>("getCurrentSystemPreference").SetResult("dark");
        var setTheme = JSInterop.SetupVoid("setTheme", "dark");

        Render<ThemeToggler>();

        setTheme.Invocations.ShouldNotBeNull().ShouldNotBeEmpty();
    }

    [Fact]
    public void ShouldSetFromLocalStorage()
    {
        var localStorage = Substitute.For<ILocalStorageService>();
        localStorage.ContainsKeyAsync("preferred-theme").Returns(true);
        localStorage.GetItemAsync<string>("preferred-theme").Returns("dark");
        Services.AddScoped(_ => localStorage);
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        JSInterop.Setup<string>("getCurrentSystemPreference").SetResult("light");
        var setTheme = JSInterop.SetupVoid("setTheme", "dark");

        Render<ThemeToggler>();

        setTheme.Invocations.ShouldNotBeNull().ShouldNotBeEmpty();
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
        var cut = Render<ThemeToggler>();

        await cut.Find("div").ClickAsync();

        setTheme.Invocations.ShouldNotBeNull().ShouldNotBeEmpty();
        await localStorage.Received(1).SetItemAsync("preferred-theme", "dark");
    }
}
