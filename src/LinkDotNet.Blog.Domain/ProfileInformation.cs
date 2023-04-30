namespace LinkDotNet.Blog.Domain;

public sealed record ProfileInformation
{
    public string Name { get; init; }

    public string Heading { get; init; }

    public string ProfilePictureUrl { get; init; }
}
