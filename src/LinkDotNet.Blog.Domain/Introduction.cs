namespace LinkDotNet.Blog.Domain;

public sealed record Introduction
{
    public const string IntroductionSection = "Introduction";

    public string? BackgroundUrl { get; init; }

    public required string ProfilePictureUrl { get; init; }

    public required string Description { get; init; }
}
