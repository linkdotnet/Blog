using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.TestUtilities;

public class IntroductionBuilder
{
    private string? backgroundUrl;
    private string profilePictureUrl = "ProfilePictureUrl";
    private string description = "Description";

    public IntroductionBuilder WithBackgroundUrl(string? backgroundUrl)
    {
        this.backgroundUrl = backgroundUrl;
        return this;
    }

    public IntroductionBuilder WithProfilePictureUrl(string profilePictureUrl)
    {
        this.profilePictureUrl = profilePictureUrl;
        return this;
    }

    public IntroductionBuilder WithDescription(string description)
    {
        this.description = description;
        return this;
    }

    public Introduction Build()
    {
        return new Introduction
        {
            BackgroundUrl = backgroundUrl,
            ProfilePictureUrl = profilePictureUrl,
            Description = description,
        };
    }
}
