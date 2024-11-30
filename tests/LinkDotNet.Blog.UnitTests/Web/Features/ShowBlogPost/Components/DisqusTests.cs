using System.Linq;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class DisqusTests : BunitContext
{
    [Fact]
    public void ShouldLoadJavascript()
    {
        var disqusData = new DisqusConfigurationBuilder()
            .WithShortName("blog")
            .Build();
        Services.AddScoped(_ => Options.Create(disqusData));
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder()
            .WithIsDisqusEnabled(true)
            .Build()));
        JSInterop.SetupModule("./Features/ShowBlogPost/Components/Disqus.razor.js");
        JSInterop.Mode = JSRuntimeMode.Loose;

        Render<Disqus>();

        var init = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "initDisqus");
        init.Arguments.ShouldContain(disqusData);
    }

    [Fact]
    public void ShouldNotInitDisqusWhenNoInformationProvided()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));

        Render<Disqus>();

        JSInterop.Invocations.ShouldBeEmpty();
    }
}