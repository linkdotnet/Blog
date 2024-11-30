using LinkDotNet.Blog.Web.Features.SupportMe.Components;

namespace LinkDotNet.Blog.TestUtilities;

public class SupportMeConfigurationBuilder
{
    private string? kofiToken = "ABC";
    private string? githubSponsorName = "linkdotnet";
    private string? patreonName = "linkdotnet";
    private bool showUnderBlogPost = true;
    private bool showUnderIntroduction = true;
    private bool showInFooter = true;
    private bool showSupportMePage = true;
    private string? supportMePageDescription = "Support me";

    public SupportMeConfigurationBuilder WithKofiToken(string? kofiToken)
    {
        this.kofiToken = kofiToken;
        return this;
    }

    public SupportMeConfigurationBuilder WithGithubSponsorName(string? githubSponsorName)
    {
        this.githubSponsorName = githubSponsorName;
        return this;
    }

    public SupportMeConfigurationBuilder WithPatreonName(string? patreonName)
    {
        this.patreonName = patreonName;
        return this;
    }

    public SupportMeConfigurationBuilder WithShowUnderBlogPost(bool showUnderBlogPost = true)
    {
        this.showUnderBlogPost = showUnderBlogPost;
        return this;
    }

    public SupportMeConfigurationBuilder WithShowUnderIntroduction(bool showUnderIntroduction = true)
    {
        this.showUnderIntroduction = showUnderIntroduction;
        return this;
    }

    public SupportMeConfigurationBuilder WithShowInFooter(bool showInFooter = true)
    {
        this.showInFooter = showInFooter;
        return this;
    }

    public SupportMeConfigurationBuilder WithShowSupportMePage(bool showSupportMePage = true)
    {
        this.showSupportMePage = showSupportMePage;
        return this;
    }

    public SupportMeConfigurationBuilder WithSupportMePageDescription(string supportMePageDescription)
    {
        this.supportMePageDescription = supportMePageDescription;
        return this;
    }

    public SupportMeConfiguration Build()
    {
        return new SupportMeConfiguration
        {
            KofiToken = kofiToken,
            GithubSponsorName = githubSponsorName,
            PatreonName = patreonName,
            ShowUnderBlogPost = showUnderBlogPost,
            ShowUnderIntroduction = showUnderIntroduction,
            ShowInFooter = showInFooter,
            ShowSupportMePage = showSupportMePage,
            SupportMePageDescription = supportMePageDescription,
        };
    }
}
