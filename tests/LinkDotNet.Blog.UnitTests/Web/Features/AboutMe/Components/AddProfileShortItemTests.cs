using Bunit;
using LinkDotNet.Blog.Web.Features.AboutMe.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe.Components;

public class AddProfileShortItemTests : TestContext
{
    [Fact]
    public void ShouldAddShortItem()
    {
        string addedItem = null;
        var cut = RenderComponent<AddProfileShortItem>(
            p => p.Add(s => s.ValueAdded, c => addedItem = c));
        cut.Find("input").Change("Key");

        cut.Find("button").Click();

        addedItem.Should().Be("Key");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ShouldNotAddItemWhenKeyOrValueIsEmpty(string content)
    {
        var wasInvoked = false;
        var cut = RenderComponent<AddProfileShortItem>(
            p => p.Add(s => s.ValueAdded, _ => wasInvoked = true));
        cut.Find("input").Change(content);

        cut.Find("button").Click();

        wasInvoked.Should().BeFalse();
    }

    [Fact]
    public void ShouldEmptyModelAfterTextEntered()
    {
        var cut = RenderComponent<AddProfileShortItem>();
        cut.Find("input").Change("Key");

        cut.Find("button").Click();

        cut.Find("input").TextContent.Should().BeEmpty();
    }
}