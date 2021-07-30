using System;
using FluentAssertions;
using LinkDotNet.Domain;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Domain
{
    public class SkillTests
    {
        [Fact]
        public void ShouldCreateSkillTrimmedWhitespaces()
        {
            var skill = Skill.Create(" C# ", "url", " Backend ", ProficiencyLevel.Expert.Key);

            skill.Name.Should().Be("C#");
            skill.IconUrl.Should().Be("url");
            skill.Capability.Should().Be("Backend");
            skill.ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldThrowWhenWhitespaceName(string name)
        {
            Action result = () => Skill.Create(name, "url", "backend", ProficiencyLevel.Expert.Key);

            result.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldThrowWhenWhitespaceCapability(string capability)
        {
            Action result = () => Skill.Create("name", "url", capability, ProficiencyLevel.Expert.Key);

            result.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldThrowWhenInvalidProficiencyLevel()
        {
            Action result = () => Skill.Create("name", "url", "cap", "null");
            result.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldSetUrlToNullWhenWhitespace(string url)
        {
            var skill = Skill.Create("name", url, "cap", ProficiencyLevel.Expert.Key);

            skill.IconUrl.Should().BeNull();
        }
    }
}