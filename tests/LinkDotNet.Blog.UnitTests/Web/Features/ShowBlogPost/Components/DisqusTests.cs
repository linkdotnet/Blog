using System.Linq;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class DisqusTests : TestContext
{
    [Fact]
    public void ShouldLoadJavascript()
    {
        var disqusData = new DisqusConfiguration()
        {
            Shortname = "blog",
        };
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration { Disqus = disqusData }));
        JSInterop.SetupModule("./Features/ShowBlogPost/Components/Disqus.razor.js");
        JSInterop.Mode = JSRuntimeMode.Loose;

        RenderComponent<Disqus>();

        var init = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "initDisqus");
        init.Should().NotBeNull();
        init.Arguments.Should().Contain(disqusData);
    }

    [Fact]
    public void ShouldNotInitDisqusWhenNoInformationProvided()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration()));

        RenderComponent<Disqus>();

        JSInterop.Invocations.Should().BeEmpty();
    }
}