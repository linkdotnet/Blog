using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared.Services;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Services;

public class MarkerServiceTests : TestContext
{
    private readonly MarkerService cut;

    public MarkerServiceTests()
    {
        cut = new MarkerService(JSInterop.JSRuntime);
    }

    [Theory]
    [InlineData("Test", 0, 2, "**", "**Te**st")]
    [InlineData("Test", 0, 4, "**", "**Test**")]
    [InlineData("Test", 0, 0, "**", "Test")]
    [InlineData("That is a test", 8, 9, "**", "That is **a** test")]
    public async Task ShouldMarkString(string source, int startSelect, int endSelect, string fence, string expected)
    {
        const string element = "id";
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.Setup<SelectionRange>("getSelectionFromElement", element)
            .SetResult(new SelectionRange { Start = startSelect, End = endSelect });

        var actual = await cut.GetNewMarkdownForElementAsync(element, source, fence, fence);

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task ShouldSetCursorPosition()
    {
        const string element = "id";
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.Setup<SelectionRange>("getSelectionFromElement", element)
            .SetResult(new SelectionRange { Start = 1, End = 3 });

        await cut.GetNewMarkdownForElementAsync(element, "Test", "**", "**");

        var setSelection = JSInterop.Invocations.SingleOrDefault(s => s.Identifier == "setSelectionFromElement");
        setSelection.Arguments.Should().Contain(element);
        setSelection.Arguments.Should().Contain(3);
    }
}