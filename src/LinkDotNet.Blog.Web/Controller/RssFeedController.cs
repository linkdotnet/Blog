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
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.Controller;

[Route("feed.rss")]
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

        numberOfBlogPosts ??= blogPostsPerPage;

        var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var introductionDescription = MarkdownConverter.ToPlainString(description);
        var feed = new SyndicationFeed(blogName, introductionDescription, new Uri(url))
        {
            Items = withContent
            ? await GetBlogPostsItemsWithContent(url, numberOfBlogPosts.Value)
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
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            Indent = true,
            Async = true,
        };
        return settings;
    }

    private static SyndicationItem CreateSyndicationItemFromBlogPost(string url, BlogPostRssInfo blogPost)
    {
        var blogPostUrl = url + $"/blogPost/{blogPost.Id}";
        var content = MarkdownConverter.ToMarkupString(blogPost.ShortDescription ?? blogPost.Content ?? string.Empty);
        var item = new SyndicationItem(
            blogPost.Title,
            default(SyndicationContent),
            new Uri(blogPostUrl),
            blogPost.Id,
            blogPost.UpdatedDate)
        {
            PublishDate = blogPost.UpdatedDate,
            LastUpdatedTime = blogPost.UpdatedDate,
            ElementExtensions =
            {
                CreateCDataElement(content.Value),
                new XElement("image", blogPost.PreviewImageUrl),
            },
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
            s => new BlogPostRssInfo(s.Id, s.Title, s.ShortDescription, null, s.UpdatedDate, s.PreviewImageUrl, s.Tags),
            f => f.IsPublished,
            orderBy: post => post.UpdatedDate);
        return blogPosts.Select(bp => CreateSyndicationItemFromBlogPost(url, bp));
    }

    private async Task<IEnumerable<SyndicationItem>> GetBlogPostsItemsWithContent(string url, int numberOfBlogPosts)
    {
        var blogPosts = await blogPostRepository.GetAllByProjectionAsync(
            s => new BlogPostRssInfo(s.Id, s.Title, null,s.Content, s.UpdatedDate, s.PreviewImageUrl, s.Tags),
            f => f.IsPublished,
            orderBy: post => post.UpdatedDate,
            pageSize: numberOfBlogPosts);
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


    private sealed record BlogPostRssInfo(
        string Id,
        string Title,
        string? ShortDescription,
        string? Content,
        DateTime UpdatedDate,
        string PreviewImageUrl,
        IEnumerable<string> Tags);
}
