using LinkDotNet.Blog.TestUtilities;
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
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().WithIsDisqusEnabled(true).Build()));
        Services.AddScoped(_ => Options.Create(new DisqusConfigurationBuilder().Build() ));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<CommentSection>();

        cut.FindComponents<Disqus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldShowGiscusWhenConfigured()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().WithIsGiscusEnabled(true).Build()));
        Services.AddScoped(_ => Options.Create(new GiscusConfigurationBuilder().Build() ));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<CommentSection>();

        cut.FindComponents<Giscus>().Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldShowAlertWhenMultipleRegistered()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder()
            .WithIsGiscusEnabled(true)
            .WithIsDisqusEnabled(true)
            .Build()));
        Services.AddScoped(_ => Options.Create( new DisqusConfigurationBuilder().Build()));
        Services.AddScoped(_ => Options.Create( new GiscusConfigurationBuilder().Build()));
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<CommentSection>();

        cut.FindAll(".alert-danger").Should().NotBeEmpty();
    }
}