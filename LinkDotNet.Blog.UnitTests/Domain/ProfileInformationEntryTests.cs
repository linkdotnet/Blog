using System;
using FluentAssertions;
using LinkDotNet.Domain;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Domain
{
    public class ProfileInformationEntryTests
    {
        [Fact]
        public void ShouldCreateObject()
        {
            var result = ProfileInformationEntry.Create("key", "value");

            result.Key.Should().Be("key");
            result.Value.Should().Be("value");
        }

        [Theory]
        [InlineData("", "V")]
        [InlineData(" ", "V")]
        [InlineData(null, "V")]
        [InlineData("K", "")]
        [InlineData("K", " ")]
        [InlineData("K", null)]
        public void ShouldThrowExceptionWhenEmptyKeyOrValue(string key, string value)
        {
            Action act = () => ProfileInformationEntry.Create(key, value);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}