using System;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class SkillTests
{
    [Fact]
    public void ShouldCreateSkillTrimmedWhitespaces()
    {
        var skill = Skill.Create(" C# ", "url", " Backend ", ProficiencyLevel.Expert.Key);

        skill.Name.ShouldBe("C#");
        skill.IconUrl.ShouldBe("url");
        skill.Capability.ShouldBe("Backend");
        skill.ProficiencyLevel.ShouldBe(ProficiencyLevel.Expert);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldThrowWhenWhitespaceName(string? name)
    {
        Action result = () => Skill.Create(name!, "url", "backend", ProficiencyLevel.Expert.Key);

        result.ShouldThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldThrowWhenWhitespaceCapability(string? capability)
    {
        Action result = () => Skill.Create("name", "url", capability!, ProficiencyLevel.Expert.Key);

        result.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowWhenInvalidProficiencyLevel()
    {
        Action result = () => Skill.Create("name", "url", "cap", "null");
        result.ShouldThrow<Exception>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldSetUrlToNullWhenWhitespace(string? url)
    {
        var skill = Skill.Create("name", url, "cap", ProficiencyLevel.Expert.Key);

        skill.IconUrl.ShouldBeNull();
    }

    [Fact]
    public void ShouldSetProficiencyLevel()
    {
        var skill = Skill.Create("name", null, "cap", ProficiencyLevel.Familiar.Key);

        skill.SetProficiencyLevel(ProficiencyLevel.Proficient);

        skill.ProficiencyLevel.ShouldBe(ProficiencyLevel.Proficient);
    }

    [Fact]
    public void ShouldThrowWhenProficiencyIsNull()
    {
        var skill = Skill.Create("name", null, "cap", ProficiencyLevel.Familiar.Key);

        Action result = () => skill.SetProficiencyLevel(null!);

        result.ShouldThrow<ArgumentNullException>();
    }
}
