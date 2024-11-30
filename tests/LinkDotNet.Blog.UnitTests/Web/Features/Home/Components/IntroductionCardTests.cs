using System.Linq;
using AngleSharp.Css.Dom;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Home.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options; 

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class IntroductionCardTests : BunitContext
{
    [Fact]
    public void ShouldSetBackgroundWhenSet()
    {
        var introduction = new IntroductionBuilder().WithBackgroundUrl("something_but_null").Build();
        
        Services.AddScoped(_ => Options.Create(introduction));

        var cut = Render<IntroductionCard>();

        var background = cut.FindAll(".introduction-background");
        background.ShouldNotBeNull().ShouldNotBeEmpty();
        background.ShouldHaveSingleItem();
        background.Single().GetStyle().CssText.ShouldContain(introduction.BackgroundUrl!);
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    public void ShouldNotSetBackgroundWhenNotSet(string? url)
    {
        var introduction = new IntroductionBuilder().WithBackgroundUrl(url).Build();

        Services.AddScoped(_ => Options.Create(introduction));

        var cut = Render<IntroductionCard>();

        var background = cut.FindAll(".introduction-background");
        background.ShouldBeEmpty();
    }
}