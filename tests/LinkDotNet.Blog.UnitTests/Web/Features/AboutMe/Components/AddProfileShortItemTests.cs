using LinkDotNet.Blog.Web.Features.AboutMe.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe.Components;

public class AddProfileShortItemTests : BunitContext
{
    [Fact]
    public void ShouldAddShortItem()
    {
        string? addedItem = null;
        var cut = Render<AddProfileShortItem>(
            p => p.Add(s => s.ValueAdded, c => addedItem = c));
        cut.Find("input").Change("Key");

        cut.Find("button").Click();

        addedItem.ShouldBe("Key");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ShouldNotAddItemWhenKeyOrValueIsEmpty(string? content)
    {
        var wasInvoked = false;
        var cut = Render<AddProfileShortItem>(
            p => p.Add(s => s.ValueAdded, _ => wasInvoked = true));
        cut.Find("input").Change(content);

        cut.Find("button").Click();

        wasInvoked.ShouldBeFalse();
    }

    [Fact]
    public void ShouldEmptyModelAfterTextEntered()
    {
        var cut = Render<AddProfileShortItem>();
        cut.Find("input").Change("Key");

        cut.Find("button").Click();

        cut.Find("input").TextContent.ShouldBeEmpty();
    }
}