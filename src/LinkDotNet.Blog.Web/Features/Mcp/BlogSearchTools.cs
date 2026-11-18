using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Search;
using Microsoft.AspNetCore.Http;
using ModelContextProtocol.Server;

namespace LinkDotNet.Blog.Web.Features.Mcp;

[McpServerToolType]
public sealed class BlogSearchTools
{
    private const int MaxResults = 20;

    private readonly IRepository<BlogPost> blogPostRepository;
    private readonly IHttpContextAccessor httpContextAccessor;

    public BlogSearchTools(IRepository<BlogPost> blogPostRepository, IHttpContextAccessor httpContextAccessor)
    {
        this.blogPostRepository = blogPostRepository;
        this.httpContextAccessor = httpContextAccessor;
    }

    [McpServerTool(Name = "search_posts", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Searches published blog posts whose title contains the query or that have the query as exact tag. Newest first. Fetch a result's url to read the full post as markdown.")]
    public Task<IReadOnlyList<BlogPostSearchResult>> SearchPostsAsync(
        [Description("Text contained in the title, or an exact tag")] string query)
        => SearchAsync(BlogPostSearch.ByTerm(query));

    [McpServerTool(Name = "search_posts_by_tag", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Lists published blog posts with the given tag. Newest first. Fetch a result's url to read the full post as markdown.")]
    public Task<IReadOnlyList<BlogPostSearchResult>> SearchPostsByTagAsync(
        [Description("Exact tag, e.g. \"C#\"")] string tag)
        => SearchAsync(BlogPostSearch.ByTag(tag));

    private async Task<IReadOnlyList<BlogPostSearchResult>> SearchAsync(Expression<Func<BlogPost, bool>> filter)
    {
        var request = httpContextAccessor.HttpContext!.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
        var blogPosts = await blogPostRepository.GetAllAsync(filter, b => b.UpdatedDate, pageSize: MaxResults);
        return blogPosts.Select(b => new BlogPostSearchResult(
                b.Title,
                GetMarkdownUrl(baseUrl, b),
                MarkdownConverter.ToPlainString(b.ShortDescription),
                [.. b.Tags],
                DateOnly.FromDateTime(b.UpdatedDate)))
            .ToArray();
    }

    private static string GetMarkdownUrl(string baseUrl, BlogPost blogPost)
        => string.IsNullOrEmpty(blogPost.Slug)
            ? $"{baseUrl}/blogPost/{blogPost.Id}.md"
            : $"{baseUrl}/blogPost/{blogPost.Id}/{blogPost.Slug}.md";
}

public sealed record BlogPostSearchResult(string Title, string Url, string? Description, IReadOnlyList<string> Tags, DateOnly Updated);
