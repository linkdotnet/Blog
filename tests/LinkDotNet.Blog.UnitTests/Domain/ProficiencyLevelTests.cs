using System;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Domain
{
    public class ProficiencyLevelTests
    {
        [Theory]
        [InlineData("Familiar")]
        [InlineData("Proficient")]
        [InlineData("Expert")]
        public void ShouldCreateValidLevels(string key)
        {
            var level = ProficiencyLevel.Create(key);

            level.Should().NotBeNull();
        }

        [Theory]
        [InlineData("NotALevel")]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldNotCreateInvalidLevels(string key)
        {
            Action act = () => ProficiencyLevel.Create(key);

            act.Should().Throw<Exception>();
        }
    }
}
