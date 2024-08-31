namespace LinkDotNet.Blog.Domain;

public sealed record ProfileInformation
{
    public const string ProfileInformationSection = "ProfileInformation";

    public required string Name { get; init; }

    public required string Heading { get; init; }

    public required string ProfilePictureUrl { get; init; }
}
