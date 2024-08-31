namespace LinkDotNet.Blog.Domain;

public sealed class ProficiencyLevel : Enumeration<ProficiencyLevel>
{
    public static readonly ProficiencyLevel Familiar = new(nameof(Familiar));
    public static readonly ProficiencyLevel Proficient = new(nameof(Proficient));
    public static readonly ProficiencyLevel Expert = new(nameof(Expert));

    private ProficiencyLevel(string key)
        : base(key)
    {
    }
}
