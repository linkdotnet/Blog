using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.TestUtilities;

public class ProfileInformationBuilder
{
    private string name = "Name";
    private string heading = "Heading";
    private string profilePictureUrl = "ProfilePictureUrl";

    public ProfileInformationBuilder WithName(string name)
    {
        this.name = name;
        return this;
    }

    public ProfileInformationBuilder WithHeading(string heading)
    {
        this.heading = heading;
        return this;
    }

    public ProfileInformationBuilder WithProfilePictureUrl(string profilePictureUrl)
    {
        this.profilePictureUrl = profilePictureUrl;
        return this;
    }

    public ProfileInformation Build()
    {
        return new ProfileInformation
        {
            Name = name,
            Heading = heading,
            ProfilePictureUrl = profilePictureUrl,
        };
    }
}
