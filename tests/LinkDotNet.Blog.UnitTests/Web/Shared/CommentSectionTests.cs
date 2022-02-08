using Bunit;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Shared;

public class CommentSectionTests : TestContext
{
    [Fact]
    public void ShouldShowDisqusWhenConfigured()
    {
        Services.AddScoped(_ => new AppConfiguration { DisqusConfiguration = new DisqusConfiguration() });
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = RenderComponent<CommentSection>();

        cut.FindComponents<Disqus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldShowGiscusWhenConfigured()
    {
        Services.AddScoped(_ => new AppConfiguration { GiscusConfiguration = new GiscusConfiguration() });
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = RenderComponent<CommentSection>();

        cut.FindComponents<Giscus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldShowAlertWhenMultipleRegistered()
    {
        Services.AddScoped(_ => new AppConfiguration
            { DisqusConfiguration = new DisqusConfiguration(), GiscusConfiguration = new GiscusConfiguration() });
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = RenderComponent<CommentSection>();

        cut.FindAll(".alert-danger").Should().NotBeEmpty();
    }
}