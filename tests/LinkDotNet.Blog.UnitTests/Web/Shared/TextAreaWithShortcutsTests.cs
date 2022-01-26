using System.Linq;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared;

public class TextAreaWithShortcutsTests : TestContext
{
    [Theory]
    [InlineData("b", 0, 4, true, "**Test**")]
    [InlineData("i", 0, 4, true, "*Test*")]
    [InlineData("h", 0, 1, true, "Test")]
    [InlineData("b", 0, 1, false, "Test")]
    [InlineData("f", 0, 4, false, "Test")]
    [InlineData("b", 0, 0, true, "Test")]
    public void ShouldSetMarkerOnKeyUp(string key, int start, int end, bool ctrlPressed, string expected)
    {
        const string id = "id";
        var range = new SelectionRange
        {
            Start = start,
            End = end,
        };
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.Setup<SelectionRange>("getSelectionFromElement", id).SetResult(range);
        var cut = RenderComponent<TextAreaWithShortcuts>(
            p => p.Add(s => s.Id, id));
        cut.Find("textarea").Input("Test");
        cut.Find("textarea").KeyUp(key, ctrlKey: ctrlPressed);

        var content = cut.Find("textarea").TextContent;

        content.Should().Be(expected);
    }

    [Fact]
    public void ShouldSetCursorPosition()
    {
        const string element = "id";
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.Setup<SelectionRange>("getSelectionFromElement", element)
            .SetResult(new SelectionRange { Start = 2, End = 5 });
        var cut = RenderComponent<TextAreaWithShortcuts>(
            p => p.Add(s => s.Id, element));
        cut.Find($"#{element}").Input("Hello World");

        cut.Find($"#{element}").KeyUp("b", ctrlKey: true);

        var setSelection = JSInterop.Invocations.SingleOrDefault(s => s.Identifier == "setSelectionFromElement");
        setSelection.Arguments.Should().Contain(element);
        setSelection.Arguments.Should().Contain(9);
    }
}