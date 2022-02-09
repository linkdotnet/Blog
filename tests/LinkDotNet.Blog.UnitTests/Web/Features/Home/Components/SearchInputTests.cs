using Bunit;
using LinkDotNet.Blog.Web.Features.Home.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Home.Components;

public class SearchInputTests : TestContext
{
    [Fact]
    public void ShouldReturnEnteredText()
    {
        var enteredString = string.Empty;
        var cut = RenderComponent<SearchInput>(p => p.Add(s => s.SearchEntered, s => enteredString = s));
        cut.Find("input").Change("Test");

        cut.Find("button").Click();

        enteredString.Should().Be("Test");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void ShouldNotReturnValueWhenOnlyWhitespaces(string input)
    {
        var wasInvoked = false;
        var cut = RenderComponent<SearchInput>(p => p.Add(s => s.SearchEntered, _ => wasInvoked = true));
        cut.Find("input").Change(input);

        cut.Find("button").Click();

        wasInvoked.Should().BeFalse();
    }

    [Fact]
    public void ShouldTrimData()
    {
        var enteredString = string.Empty;
        var cut = RenderComponent<SearchInput>(p => p.Add(s => s.SearchEntered, s => enteredString = s));
        cut.Find("input").Change("   Test 1 ");

        cut.Find("button").Click();

        enteredString.Should().Be("Test 1");
    }

    [Theory]
    [InlineData("Enter", true)]
    [InlineData("Escape", false)]
    [InlineData("Backspace", false)]
    public void ShouldReturnValueWhenEnterWasPressed(string key, bool expectedInvoke)
    {
        var wasInvoked = false;
        var cut = RenderComponent<SearchInput>(p => p.Add(s => s.SearchEntered, _ => wasInvoked = true));
        cut.Find("input").Change("Text");

        cut.Find("input").KeyUp(Key.Get(key));

        wasInvoked.Should().Be(expectedInvoke);
    }
}