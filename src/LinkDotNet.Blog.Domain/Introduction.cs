namespace LinkDotNet.Blog.Domain;

public sealed record Introduction
{
    public const string IntroductionSection = "Introduction";

    public string BackgroundUrl { get; init; }

    public string ProfilePictureUrl { get; init; }

    public string Description { get; init; }
}
