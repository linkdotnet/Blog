using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

namespace LinkDotNet.Blog.TestUtilities;

public class GiscusConfigurationBuilder
{
    private string repository = "LinkDotNet/Blog";
    private string repositoryId = "MDEwOlJlcG9zaXRvcnkzNzYwMjYwNzA=";
    private string category = "General";
    private string categoryId = "MDEwOlJlcG9zaXRvcnkzNzYwMjYwNzA=";

    public GiscusConfigurationBuilder WithRepository(string repository)
    {
        this.repository = repository;
        return this;
    }

    public GiscusConfigurationBuilder WithRepositoryId(string repositoryId)
    {
        this.repositoryId = repositoryId;
        return this;
    }

    public GiscusConfigurationBuilder WithCategory(string category)
    {
        this.category = category;
        return this;
    }

    public GiscusConfigurationBuilder WithCategoryId(string categoryId)
    {
        this.categoryId = categoryId;
        return this;
    }

    public GiscusConfiguration Build()
    {
        return new GiscusConfiguration
        {
            Repository = repository,
            RepositoryId = repositoryId,
            Category = category,
            CategoryId = categoryId,
        };
    }
}
