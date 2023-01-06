using LinkDotNet.Blog.Web.Features.Home.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class ThemeTogglerTests : TestContext
{
    [Fact]
    public void ShouldSetSystemDefault()
    {
        Services.AddScoped(_ => Mock.Of<ILocalStorageService>());
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        JSInterop.Setup<string>("getCurrentSystemPreference").SetResult("dark");
        var setTheme = JSInterop.SetupVoid("setTheme", "dark");

        RenderComponent<ThemeToggler>();

        setTheme.Invocations.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldSetFromLocalStorage()
    {
        var localStorage = new Mock<ILocalStorageService>();
        localStorage.Setup(l => l.ContainKeyAsync("preferred-theme"))
            .ReturnsAsync(true);
        localStorage.Setup(l => l.GetItemAsync<string>("preferred-theme"))
            .ReturnsAsync("dark");
        Services.AddScoped(_ => localStorage.Object);
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        JSInterop.Setup<string>("getCurrentSystemPreference").SetResult("light");
        var setTheme = JSInterop.SetupVoid("setTheme", "dark");

        RenderComponent<ThemeToggler>();

        setTheme.Invocations.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldSetValueAndSafeInStorage()
    {
        var localStorage = new Mock<ILocalStorageService>();
        Services.AddScoped(_ => localStorage.Object);
        JSInterop.SetupModule("./Features/Home/Components/ThemeToggler.razor.js");
        JSInterop.Setup<string>("getCurrentSystemPreference").SetResult("light");
        JSInterop.SetupVoid("setTheme", "light");
        var setTheme = JSInterop.SetupVoid("setTheme", "dark")
            .SetVoidResult();
        var cut = RenderComponent<ThemeToggler>();

        cut.Find("span").Click();

        setTheme.Invocations.Should().NotBeNullOrEmpty();
        localStorage.Verify(l => l.SetItemAsync("preferred-theme", "dark"));
    }
}
