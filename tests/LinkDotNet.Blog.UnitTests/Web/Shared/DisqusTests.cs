using System.Linq;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Blog.Web.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared;

public class DisqusTests : TestContext
{
    [Fact]
    public void ShouldLoadJavascript()
    {
        var disqusData = new DisqusConfiguration()
        {
            Shortname = "blog",
        };
        Services.AddScoped(_ => new AppConfiguration { DisqusConfiguration = disqusData });
        JSInterop.SetupModule("./Shared/Disqus.razor.js");
        JSInterop.Mode = JSRuntimeMode.Loose;

        RenderComponent<Disqus>();

        var init = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "initDisqus");
        init.Should().NotBeNull();
        init.Arguments.Should().Contain(disqusData);
    }

    [Fact]
    public void ShouldNotInitDisqusWhenNoInformationProvided()
    {
        Services.AddScoped(_ => new AppConfiguration());

        RenderComponent<Disqus>();

        JSInterop.Invocations.Should().BeEmpty();
    }
}