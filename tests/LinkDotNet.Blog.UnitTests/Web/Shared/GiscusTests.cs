using System.Linq;
using Bunit;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Shared;

public class GiscusTests : TestContext
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
        Services.AddScoped(_ => new AppConfiguration { GiscusConfiguration = giscusData });
        JSInterop.SetupModule("./Shared/Giscus.razor.js");
        JSInterop.Mode = JSRuntimeMode.Loose;

        RenderComponent<Giscus>();

        var init = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "initGiscus");
        init.Should().NotBeNull();
        init.Arguments.Should().Contain("giscus");
        init.Arguments.Should().Contain(giscusData);
    }

    [Fact]
    public void ShouldNotInitGiscusWhenNoInformationProvided()
    {
        Services.AddScoped(_ => new AppConfiguration());

        RenderComponent<Giscus>();

        JSInterop.Invocations.Should().BeEmpty();
    }
}