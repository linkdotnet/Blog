using LinkDotNet.Blog.Web;

namespace LinkDotNet.Blog.TestUtilities;

public class ApplicationConfigurationBuilder
{
    private string blogName = "Test";
    private string connectionString = "file:local.db";
    private string databaseName = "Blog";
    private int blogPostsPerPage = 10;
    private int firstPageCacheDurationInMinutes = 5;
    private bool isAboutMeEnabled;
    private bool isGiscusEnabled;
    private bool isDisqusEnabled;
    private bool showReadingIndicator;
    private bool showSimilarPosts;
    private string? blogBrandUrl;

    public ApplicationConfigurationBuilder WithBlogName(string blogName)
    {
        this.blogName = blogName;
        return this;
    }

    public ApplicationConfigurationBuilder WithConnectionString(string connectionString)
    {
        this.connectionString = connectionString;
        return this;
    }

    public ApplicationConfigurationBuilder WithDatabaseName(string databaseName)
    {
        this.databaseName = databaseName;
        return this;
    }

    public ApplicationConfigurationBuilder WithBlogPostsPerPage(int blogPostsPerPage)
    {
        this.blogPostsPerPage = blogPostsPerPage;
        return this;
    }

    public ApplicationConfigurationBuilder WithFirstPageCacheDurationInMinutes(int firstPageCacheDurationInMinutes)
    {
        this.firstPageCacheDurationInMinutes = firstPageCacheDurationInMinutes;
        return this;
    }

    public ApplicationConfigurationBuilder WithIsAboutMeEnabled(bool isAboutMeEnabled)
    {
        this.isAboutMeEnabled = isAboutMeEnabled;
        return this;
    }

    public ApplicationConfigurationBuilder WithIsGiscusEnabled(bool isGiscusEnabled)
    {
        this.isGiscusEnabled = isGiscusEnabled;
        return this;
    }

    public ApplicationConfigurationBuilder WithIsDisqusEnabled(bool isDisqusEnabled)
    {
        this.isDisqusEnabled = isDisqusEnabled;
        return this;
    }

    public ApplicationConfigurationBuilder WithShowReadingIndicator(bool showReadingIndicator)
    {
        this.showReadingIndicator = showReadingIndicator;
        return this;
    }
    
    public ApplicationConfigurationBuilder WithShowSimilarPosts(bool showSimilarPosts)
    {
        this.showSimilarPosts = showSimilarPosts;
        return this;
    }
    
    public ApplicationConfigurationBuilder WithBlogBrandUrl(string? blogBrandUrl)
    {
        this.blogBrandUrl = blogBrandUrl;
        return this;
    }
    
    public ApplicationConfiguration Build()
    {
        return new ApplicationConfiguration
        {
            BlogName = blogName,
            ConnectionString = connectionString,
            DatabaseName = databaseName,
            BlogPostsPerPage = blogPostsPerPage,
            FirstPageCacheDurationInMinutes = firstPageCacheDurationInMinutes,
            IsAboutMeEnabled = isAboutMeEnabled,
            IsGiscusEnabled = isGiscusEnabled,
            IsDisqusEnabled = isDisqusEnabled,
            ShowReadingIndicator = showReadingIndicator,
            ShowSimilarPosts = showSimilarPosts,
            BlogBrandUrl = blogBrandUrl,
        };
    }
}
