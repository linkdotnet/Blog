using System;
using System.Collections.Generic;
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

namespace LinkDotNet.Blog.Web.Controller;

public class RssFeedController : ControllerBase
{
    private readonly AppConfiguration appConfiguration;
    private readonly IRepository<BlogPost> blogPostRepository;

    public RssFeedController(AppConfiguration appConfiguration, IRepository<BlogPost> blogPostRepository)
    {
        this.appConfiguration = appConfiguration;
        this.blogPostRepository = blogPostRepository;
    }

    [ResponseCache(Duration = 1200)]
    [HttpGet]
    [Route("feed.rss")]
    public async Task<IActionResult> GetRssFeed()
    {
        var url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var introductionDescription = MarkdownConverter.ToPlainString(appConfiguration.Introduction?.Description);
        var feed = new SyndicationFeed(appConfiguration.BlogName, introductionDescription, new Uri(url))
        {
            Items = await GetBlogPostItems(url),
        };

        var settings = CreateXmlWriterSettings();

        using var stream = new MemoryStream();
        await using var xmlWriter = XmlWriter.Create(stream, settings);
        var rssFormatter = new Rss20FeedFormatter(feed, false);
        rssFormatter.WriteTo(xmlWriter);
        await xmlWriter.FlushAsync();

        return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
    }

    private static XmlWriterSettings CreateXmlWriterSettings()
    {
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true,
            Async = true,
        };
        return settings;
    }

    private static SyndicationItem CreateSyndicationItemFromBlogPost(string url, BlogPost blogPost)
    {
        var blogPostUrl = url + $"/blogPost/{blogPost.Id}";
        var shortDescription = MarkdownConverter.ToPlainString(blogPost.ShortDescription).Trim();
        var item = new SyndicationItem(
            blogPost.Title,
            shortDescription,
            new Uri(blogPostUrl),
            blogPost.Id,
            blogPost.UpdatedDate)
        {
            PublishDate = blogPost.UpdatedDate,
            LastUpdatedTime = blogPost.UpdatedDate,
            ElementExtensions = { new XElement("image", blogPost.PreviewImageUrl) },
        };
        return item;
    }

    private async Task<List<SyndicationItem>> GetBlogPostItems(string url)
    {
        var blogPostItems = new List<SyndicationItem>();
        var blogPosts = await blogPostRepository.GetAllAsync(f => f.IsPublished, orderBy: post => post.UpdatedDate);
        blogPostItems.AddRange(blogPosts.Select(bp => CreateSyndicationItemFromBlogPost(url, bp)));

        return blogPostItems;
    }
}