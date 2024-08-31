using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.TestUtilities;

public class DisqusConfigurationBuilder
{
    private string shortName = "linkdotnet";

    public DisqusConfigurationBuilder WithShortName(string shortName)
    {
        this.shortName = shortName;
        return this;
    }

    public DisqusConfiguration Build()
    {
        return new DisqusConfiguration
        {
            Shortname = shortName,
        };
    }
}
