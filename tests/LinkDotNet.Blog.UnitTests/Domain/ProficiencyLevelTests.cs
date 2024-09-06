using System;
using LinkDotNet.Blog.Domain;

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

            level.ShouldNotBeNull();
        }

        [Theory]
        [InlineData("NotALevel")]
        [InlineData("")]
        [InlineData(null)]
        public void ShouldNotCreateInvalidLevels(string key)
        {
            Action act = () => ProficiencyLevel.Create(key);

            act.ShouldThrow<Exception>();
        }
    }
}
