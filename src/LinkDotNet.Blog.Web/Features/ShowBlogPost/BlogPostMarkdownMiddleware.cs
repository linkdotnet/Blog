using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace LinkDotNet.Blog.Web.Features.ShowBlogPost;

public sealed class BlogPostMarkdownMiddleware
{
    private const string MarkdownMediaType = "text/markdown";
    private static readonly string[] HtmlMediaTypes = ["text/html", "text/*", "*/*"];

    private readonly RequestDelegate next;

    public BlogPostMarkdownMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IOptions<ApplicationConfiguration> appConfiguration,
        IRepository<BlogPost> blogPostRepository,
        IRepository<ShortCode> shortCodeRepository)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(appConfiguration);
        ArgumentNullException.ThrowIfNull(blogPostRepository);
        ArgumentNullException.ThrowIfNull(shortCodeRepository);

        if (!HttpMethods.IsGet(context.Request.Method) || !IsBlogPostPage(context))
        {
            await next(context);
            return;
        }

        context.Response.Headers.Append(HeaderNames.Vary, HeaderNames.Accept);
        if (!PrefersMarkdown(context.Request.GetTypedHeaders().Accept))
        {
            await next(context);
            return;
        }

        // A content type keeps UseStatusCodePagesWithReExecute from swapping the 404 for the HTML NotFound page
        context.Response.ContentType = $"{MarkdownMediaType}; charset=utf-8";
        var blogPost = await blogPostRepository.GetByIdAsync(context.Request.RouteValues["blogPostId"]?.ToString() ?? string.Empty);
        if (blogPost is not { IsPublished: true })
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("# Not found\n", context.RequestAborted);
            return;
        }

        var shortCodes = await shortCodeRepository.GetAllAsync();
        var markdown = BlogPostMarkdownRenderer.Render(blogPost, shortCodes, GetCanonicalUrl(context.Request, blogPost), appConfiguration.Value.UseMultiAuthorMode);
        await context.Response.WriteAsync(markdown, context.RequestAborted);
    }

    public static bool PrefersMarkdown(IList<MediaTypeHeaderValue> accept)
    {
        ArgumentNullException.ThrowIfNull(accept);

        var markdownQuality = GetQuality(accept, MarkdownMediaType) ?? 0;
        // RFC 9110 12.5.1: the most specific matching range wins, so "text/html;q=0, */*" still rejects HTML
        var htmlQuality = HtmlMediaTypes.Select(m => GetQuality(accept, m)).FirstOrDefault(q => q.HasValue) ?? 0;
        return markdownQuality > 0 && markdownQuality >= htmlQuality;
    }

    private static double? GetQuality(IList<MediaTypeHeaderValue> accept, string mediaType)
        => accept
            .Where(a => a.MediaType.Equals(mediaType, StringComparison.OrdinalIgnoreCase))
            .Select(a => (double?)(a.Quality ?? 1))
            .Max();

    private static bool IsBlogPostPage(HttpContext context)
        => context.GetEndpoint()?.Metadata.GetMetadata<ComponentTypeMetadata>()?.Type == typeof(ShowBlogPostPage);

    private static string GetCanonicalUrl(HttpRequest request, BlogPost blogPost)
    {
        var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}/blogPost/{blogPost.Id}";
        return string.IsNullOrEmpty(blogPost.Slug) ? baseUrl : $"{baseUrl}/{blogPost.Slug}";
    }
}
