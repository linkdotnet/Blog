using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class CommentSectionTests : TestContext
{
    [Fact]
    public void ShouldShowDisqusWhenConfigured()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration { Disqus = new DisqusConfiguration() }));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = RenderComponent<CommentSection>();

        cut.FindComponents<Disqus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldShowGiscusWhenConfigured()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration { Giscus = new GiscusConfiguration() }));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = RenderComponent<CommentSection>();

        cut.FindComponents<Giscus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldShowAlertWhenMultipleRegistered()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration
            { Disqus = new DisqusConfiguration(), Giscus = new GiscusConfiguration() }));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = RenderComponent<CommentSection>();

        cut.FindAll(".alert-danger").Should().NotBeEmpty();
    }
}