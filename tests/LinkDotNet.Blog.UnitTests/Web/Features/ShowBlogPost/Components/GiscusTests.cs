using System.Linq;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class GiscusTests : BunitContext
{
    [Fact]
    public void ShouldLoadJavascript()
    {
        var giscusData = new GiscusConfiguration
        {
            Repository = "linkdotnet/somerepo",
            RepositoryId = "some_repo_id",
            Category = "General",
            CategoryId = "GeneralId",
        };
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().WithIsGiscusEnabled(true).Build()));
        Services.AddScoped(_ => Options.Create(giscusData));
        JSInterop.SetupModule("./Features/ShowBlogPost/Components/Giscus.razor.js");
        JSInterop.Mode = JSRuntimeMode.Loose;

        Render<Giscus>();

        var init = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "initGiscus");
        init.Should().NotBeNull();
        init.Arguments.Should().Contain("giscus");
        init.Arguments.Should().Contain(giscusData);
    }

    [Fact]
    public void ShouldNotInitGiscusWhenNoInformationProvided()
    {
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));

        Render<Giscus>();

        JSInterop.Invocations.Should().BeEmpty();
    }
}