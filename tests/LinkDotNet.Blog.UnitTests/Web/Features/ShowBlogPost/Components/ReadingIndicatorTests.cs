using System.Linq;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;

public class ReadingIndicatorTests : BunitContext
{
    [Fact]
    public void ShouldLoadJavascript()
    {
        JSInterop.SetupModule("./Features/ShowBlogPost/Components/ReadingIndicator.razor.js");
        JSInterop.Mode = JSRuntimeMode.Loose;

        Render<ReadingIndicator>(parameters => parameters
            .Add(p => p.ContainerCssSelector, ".blog-inner-content"));

        var init = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "initCircularReadingProgress");

        init.Arguments.ShouldContain(".blog-inner-content");
    }
}
