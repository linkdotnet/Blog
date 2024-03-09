using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class CommentSectionTests : BunitContext
{
    [Fact]
    public void ShouldShowDisqusWhenConfigured()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration{IsDisqusEnabled = true} ));
        Services.AddScoped(_ => Options.Create(new DisqusConfiguration() ));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<CommentSection>();

        cut.FindComponents<Disqus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldShowGiscusWhenConfigured()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration{IsGiscusEnabled = true} ));
        Services.AddScoped(_ => Options.Create(new GiscusConfiguration() ));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<CommentSection>();

        cut.FindComponents<Giscus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldShowAlertWhenMultipleRegistered()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfiguration
            { IsDisqusEnabled = true, IsGiscusEnabled = true }));
        Services.AddScoped(_ => Options.Create( new DisqusConfiguration()));
        Services.AddScoped(_ => Options.Create( new GiscusConfiguration()));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<CommentSection>();

        cut.FindAll(".alert-danger").Should().NotBeEmpty();
    }
}