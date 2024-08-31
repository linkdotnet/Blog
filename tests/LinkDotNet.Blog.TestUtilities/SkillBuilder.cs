using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.TestUtilities;

public class SkillBuilder
{
    private string skill = "C#";
    private string? iconUrl;
    private string capability = "Backend";
    private ProficiencyLevel proficiencyLevel = ProficiencyLevel.Familiar;

    public SkillBuilder WithSkillName(string skill)
    {
        this.skill = skill;
        return this;
    }

    public SkillBuilder WithIconUrl(string iconUrl)
    {
        this.iconUrl = iconUrl;
        return this;
    }

    public SkillBuilder WithCapability(string capability)
    {
        this.capability = capability;
        return this;
    }

    public SkillBuilder WithProficiencyLevel(ProficiencyLevel proficiencyLevel)
    {
        this.proficiencyLevel = proficiencyLevel;
        return this;
    }

    public Skill Build()
    {
        return Skill.Create(skill, iconUrl, capability, proficiencyLevel.Key);
    }
}
