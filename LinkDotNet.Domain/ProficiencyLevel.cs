namespace LinkDotNet.Domain
{
    public class ProficiencyLevel : Enumeration<ProficiencyLevel>
    {
        public static readonly ProficiencyLevel Familiar = new(nameof(Familiar));
        public static readonly ProficiencyLevel Proficient = new(nameof(Proficient));
        public static readonly ProficiencyLevel Expert = new(nameof(Expert));

        public ProficiencyLevel(string key)
            : base(key)
        {
        }
    }
}