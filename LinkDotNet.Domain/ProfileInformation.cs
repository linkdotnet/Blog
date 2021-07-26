namespace LinkDotNet.Domain
{
    public record ProfileInformation
    {
        public string Name { get; init; }

        public string Heading { get; init; }

        public string ProfilePictureUrl { get; init; }
    }
}