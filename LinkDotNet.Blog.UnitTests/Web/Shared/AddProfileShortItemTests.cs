using System.Collections.Generic;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
    public class AddProfileShortItemTests : TestContext
    {
        [Fact]
        public void ShouldAddShortItem()
        {
            KeyValuePair<string, string> addedItem = default;
            var cut = RenderComponent<AddProfileShortItem>(
                p => p.Add(s => s.ValueAdded, pair => addedItem = pair));
            cut.FindAll("input")[0].Change("Key");
            cut.FindAll("input")[1].Change("Value");

            cut.Find("button").Click();

            addedItem.Key.Should().Be("Key");
            addedItem.Value.Should().Be("Value");
        }

        [Theory]
        [InlineData("", "v")]
        [InlineData(" ", "v")]
        [InlineData(null, "v")]
        [InlineData("k", "")]
        [InlineData("k", " ")]
        [InlineData("k", null)]
        public void ShouldNotAddItemWhenKeyOrValueIsEmpty(string key, string value)
        {
            var wasInvoked = false;
            var cut = RenderComponent<AddProfileShortItem>(
                p => p.Add(s => s.ValueAdded, _ => wasInvoked = true));
            cut.FindAll("input")[0].Change(key);
            cut.FindAll("input")[1].Change(value);

            cut.Find("button").Click();

            wasInvoked.Should().BeFalse();
        }

        [Fact]
        public void ShouldEmptyModelAfterTextEntered()
        {
            var cut = RenderComponent<AddProfileShortItem>();
            cut.FindAll("input")[0].Change("Key");
            cut.FindAll("input")[1].Change("Value");

            cut.Find("button").Click();

            cut.FindAll("input")[0].TextContent.Should().BeEmpty();
            cut.FindAll("input")[1].TextContent.Should().BeEmpty();
        }
    }
}