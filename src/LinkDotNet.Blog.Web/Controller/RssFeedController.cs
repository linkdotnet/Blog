using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
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
        var feed = new SyndicationFeed(appConfiguration.BlogName, appConfiguration.Introduction?.Description, new Uri(url))
        {
            Items = await GetBlogPostItems(url),
        };

        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true,
            Async = true,
        };

        using var stream = new MemoryStream();
        await using var xmlWriter = XmlWriter.Create(stream, settings);
        var rssFormatter = new Rss20FeedFormatter(feed, false);
        rssFormatter.WriteTo(xmlWriter);
        await xmlWriter.FlushAsync();

        return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
    }

    private async Task<List<SyndicationItem>> GetBlogPostItems(string url)
    {
        var blogPostItems = new List<SyndicationItem>();
        var blogPosts = await blogPostRepository.GetAllAsync(f => f.IsPublished, orderBy: post => post.UpdatedDate);
        foreach (var blogPost in blogPosts)
        {
            var blogPostUrl = url + $"/blogPost/{blogPost.Id}";
            var item = new SyndicationItem(blogPost.Title, blogPost.ShortDescription, new Uri(blogPostUrl), blogPost.Id, blogPost.UpdatedDate)
            {
                PublishDate = blogPost.UpdatedDate,
                LastUpdatedTime = blogPost.UpdatedDate,
                ElementExtensions = { new XElement("image", blogPost.PreviewImageUrl) },
            };
            blogPostItems.Add(item);
        }

        return blogPostItems;
    }
}