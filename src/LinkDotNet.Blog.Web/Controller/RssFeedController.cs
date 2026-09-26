using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using InvalidOperationException = System.InvalidOperationException;

namespace LinkDotNet.Blog.Web.Controller;

[Route("feed.rss")]
[EnableRateLimiting("ip")]
public sealed class RssFeedController : ControllerBase
{
    private static readonly XmlWriterSettings Settings = CreateXmlWriterSettings();
    private readonly string description;
    private readonly string blogName;
    private readonly int blogPostsPerPage;
    private readonly IRepository<BlogPost> blogPostRepository;

    public RssFeedController(
        IOptions<Introduction> introductionConfiguration,
        IOptions<ApplicationConfiguration> applicationConfiguration,
        IRepository<BlogPost> blogPostRepository)
    {
        ArgumentNullException.ThrowIfNull(introductionConfiguration);
        ArgumentNullException.ThrowIfNull(applicationConfiguration);

        description = introductionConfiguration.Value.Description;
        blogName = applicationConfiguration.Value.BlogName;
        blogPostsPerPage = applicationConfiguration.Value.BlogPostsPerPage;
        this.blogPostRepository = blogPostRepository;
    }

    [ResponseCache(Duration = 1200)]
    [HttpGet]
    public async Task<IActionResult> GetRssFeed([FromQuery] bool withContent = false, [FromQuery] int? numberOfBlogPosts = null)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var introductionDescription = MarkdownConverter.ToPlainString(description);
        var feed = new SyndicationFeed(blogName, introductionDescription, new Uri(url))
        {
            Items = withContent
                ? await GetBlogPostsItemsWithContent(url, numberOfBlogPosts)
                : await GetBlogPostItems(url),
        };

        using var stream = new MemoryStream();
        await WriteRssInfoToStreamAsync(stream, feed);

        return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
    }

    private static async Task WriteRssInfoToStreamAsync(Stream stream, SyndicationFeed feed)
    {
        await using var xmlWriter = XmlWriter.Create(stream, Settings);
        var rssFormatter = new Rss20FeedFormatter(feed, false);
        rssFormatter.WriteTo(xmlWriter);
        await xmlWriter.FlushAsync();
    }

    private static XmlWriterSettings CreateXmlWriterSettings()
    {
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8, NewLineHandling = NewLineHandling.Entitize, Indent = true, Async = true,
        };
        return settings;
    }

    private static SyndicationItem CreateSyndicationItemFromBlogPost(string url, BlogPostRssInfo blogPost)
    {
        var blogPostUrl = $"{url}/{BlogPostRoute.For(blogPost.Id)}";

        var content = MarkdownConverter.ToMarkupString(blogPost.ShortDescription ?? blogPost.Content ??
            throw new InvalidOperationException("Blog post must have either short description or content."));

        var item = new SyndicationItem(
            blogPost.Title,
            default(SyndicationContent),
            new Uri(blogPostUrl),
            blogPost.Id,
            blogPost.UpdatedDate)
        {
            PublishDate = blogPost.UpdatedDate,
            LastUpdatedTime = blogPost.UpdatedDate,
            ElementExtensions = { CreateCDataElement(content.Value), new XElement("image", blogPost.PreviewImageUrl), },
        };

        AddCategories(item.Categories, blogPost);
        return item;
    }

    private static void AddCategories(Collection<SyndicationCategory> categories, BlogPostRssInfo blogPost)
    {
        foreach (var tag in blogPost.Tags ?? [])
        {
            categories.Add(new SyndicationCategory(tag));
        }
    }

    private async Task<IEnumerable<SyndicationItem>> GetBlogPostItems(string url)
    {
        var blogPosts = await blogPostRepository.GetAllByProjectionAsync(
            s => new BlogPostRssInfo { Id = s.Id, Title = s.Title, ShortDescription = s.ShortDescription, UpdatedDate = s.UpdatedDate, PreviewImageUrl = s.PreviewImageUrl, Tags = s.Tags },
            f => f.IsPublished,
            orderBy: post => post.UpdatedDate);
        return blogPosts.Select(bp => CreateSyndicationItemFromBlogPost(url, bp));
    }

    private async Task<IEnumerable<SyndicationItem>> GetBlogPostsItemsWithContent(string url, int? numberOfBlogPosts)
    {
        numberOfBlogPosts ??= blogPostsPerPage;

        var blogPosts = await blogPostRepository.GetAllByProjectionAsync(
            s => new BlogPostRssInfo { Id = s.Id, Title = s.Title, Content = s.Content, UpdatedDate = s.UpdatedDate, PreviewImageUrl = s.PreviewImageUrl, Tags = s.Tags },
            f => f.IsPublished,
            orderBy: post => post.UpdatedDate,
            pageSize: numberOfBlogPosts.Value);
        return blogPosts.Select(bp => CreateSyndicationItemFromBlogPost(url, bp));
    }

    private static XmlElement CreateCDataElement(string htmlContent)
    {
        var doc = new XmlDocument();
        var cdataSection = doc.CreateCDataSection(htmlContent);
        var element = doc.CreateElement("description");
        element.AppendChild(cdataSection);
        return element;
    }


    // Member-init on purpose: RavenDB can't project into constructors with parameters.
    private sealed record BlogPostRssInfo
    {
        public required string Id { get; init; }

        public required string Title { get; init; }

        public string? ShortDescription { get; init; }

        public string? Content { get; init; }

        public DateTime UpdatedDate { get; init; }

        public required string PreviewImageUrl { get; init; }

        public required IEnumerable<string> Tags { get; init; }
    }
}
