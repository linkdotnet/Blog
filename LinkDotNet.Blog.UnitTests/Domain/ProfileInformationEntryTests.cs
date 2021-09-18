using System;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Domain
{
    public class ProfileInformationEntryTests
    {
        [Fact]
        public void ShouldCreateObject()
        {
            var result = ProfileInformationEntry.Create("key", 12);

            result.Content.Should().Be("key");
            result.SortOrder.Should().Be(12);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ShouldThrowExceptionWhenEmptyKeyOrValue(string content)
        {
            Action act = () => ProfileInformationEntry.Create(content, 0);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ShouldTrim()
        {
            var result = ProfileInformationEntry.Create("   key ", 0);

            result.Content.Should().Be("key");
        }
    }
}