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
            var result = ProfileInformationEntry.Create("key");

            result.Content.Should().Be("key");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void ShouldThrowExceptionWhenEmptyKeyOrValue(string content)
        {
            Action act = () => ProfileInformationEntry.Create(content);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ShouldTrim()
        {
            var result = ProfileInformationEntry.Create("   key ");

            result.Content.Should().Be("key");
        }
    }
}