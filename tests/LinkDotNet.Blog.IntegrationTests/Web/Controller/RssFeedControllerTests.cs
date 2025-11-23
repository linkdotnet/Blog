using System;
using System.Text;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.IntegrationTests.Web.Controller;

public class RssFeedControllerTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldCreateRssFeed()
    {
        var request = Substitute.For<HttpRequest>();
        request.Scheme.Returns("http");
        request.Host.Returns(new HostString("localhost"));
        request.PathBase.Returns(PathString.FromUriComponent("/"));
        var httpContext = Substitute.For<HttpContext>();
        httpContext.Request.Returns(request);
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };
        var config = Options.Create(new ApplicationConfigurationBuilder()
            .WithBlogName("Test")
            .Build());

        var introduction = new IntroductionBuilder()
            .WithDescription("Description")
            .Build();
        var introductionConfig = Options.Create(introduction);
        var blogPost1 = new BlogPostBuilder()
            .WithTitle("1")
            .WithShortDescription("Short 1")
            .WithPreviewImageUrl("preview1")
            .WithUpdatedDate(new DateTime(2022, 5, 1, 0, 0, 0, DateTimeKind.Utc))
            .WithTags("C#", ".NET")
            .Build();
        var blogPost2 = new BlogPostBuilder()
            .WithTitle("2")
            .WithShortDescription("**Short 2**")
            .WithPreviewImageUrl("preview2")
            .WithUpdatedDate(new DateTime(2022, 6, 1, 0, 0, 0, DateTimeKind.Utc))
            .Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        var cut = new RssFeedController(introductionConfig, config, Repository)
        {
            ControllerContext = controllerContext,
        };

        var xml = await cut.GetRssFeed() as FileContentResult;

        xml.ShouldNotBeNull();
        var content = Encoding.UTF8.GetString(xml.FileContents);
        content.ShouldMatch(
            $"""
            .*
            <rss version="2.0">
              <channel>
                <title>Test</title>
                <link>http://localhost/</link>
                <description>Description</description>
                <item>
                  <guid isPermaLink="false">{blogPost2.Id}</guid>
                  <link>http://localhost//blogPost/{blogPost2.Id}</link>
                  <title>2</title>
                  <pubDate>Wed, 01 Jun 2022 00:00:00.*</pubDate>
                  <description><!\[CDATA\[<p><strong>Short 2</strong></p>
            ]]></description>
                  <image>preview2</image>
                </item>
                <item>
                  <guid isPermaLink="false">{blogPost1.Id}</guid>
                  <link>http://localhost//blogPost/{blogPost1.Id}</link>
                  <category>C#</category>
                  <category>.NET</category>
                  <title>1</title>
                  <pubDate>Sun, 01 May 2022 00:00:00.*</pubDate>
                  <description><!\[CDATA\[<p>Short 1</p>
            ]]></description>
                  <image>preview1</image>
                </item>
              </channel>
            </rss>
            """);
    }
    
    [Fact]
    public async Task ShouldReturnFullContentIfRequested()
    {
        var request = Substitute.For<HttpRequest>();
        request.Scheme.Returns("http");
        request.Host.Returns(new HostString("localhost"));
        request.PathBase.Returns(PathString.FromUriComponent("/"));
        var httpContext = Substitute.For<HttpContext>();
        httpContext.Request.Returns(request);
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };
        var config = Options.Create(new ApplicationConfigurationBuilder()
            .WithBlogName("Test")
            .Build());

        var introduction = new IntroductionBuilder()
            .WithDescription("Description")
            .Build();
        var introductionConfig = Options.Create(introduction);
        var blogPost1 = new BlogPostBuilder()
            .WithTitle("1")
            .WithContent("Content1")
            .WithPreviewImageUrl("preview1")
            .WithUpdatedDate(new DateTime(2022, 5, 1, 0, 0, 0, DateTimeKind.Utc))
            .WithTags("C#", ".NET")
            .Build();
        var blogPost2 = new BlogPostBuilder()
            .WithTitle("2")
            .WithContent("**Content 2**")
            .WithPreviewImageUrl("preview2")
            .WithUpdatedDate(new DateTime(2022, 6, 1, 0, 0, 0, DateTimeKind.Utc))
            .Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        var cut = new RssFeedController(introductionConfig, config, Repository)
        {
            ControllerContext = controllerContext,
        };

        var xml = await cut.GetRssFeed(withContent: true) as FileContentResult;

        xml.ShouldNotBeNull();
        var content = Encoding.UTF8.GetString(xml.FileContents);
        content.ShouldMatch(
            $"""
             .*
             <rss version="2.0">
               <channel>
                 <title>Test</title>
                 <link>http://localhost/</link>
                 <description>Description</description>
                 <item>
                   <guid isPermaLink="false">{blogPost2.Id}</guid>
                   <link>http://localhost//blogPost/{blogPost2.Id}</link>
                   <title>2</title>
                   <pubDate>Wed, 01 Jun 2022 00:00:00.*</pubDate>
                   <description><!\[CDATA\[<p><strong>Content 2</strong></p>
             ]]></description>
                   <image>preview2</image>
                 </item>
                 <item>
                   <guid isPermaLink="false">{blogPost1.Id}</guid>
                   <link>http://localhost//blogPost/{blogPost1.Id}</link>
                   <category>C#</category>
                   <category>.NET</category>
                   <title>1</title>
                   <pubDate>Sun, 01 May 2022 00:00:00.*</pubDate>
                   <description><!\[CDATA\[<p>Content1</p>
             ]]></description>
                   <image>preview1</image>
                 </item>
               </channel>
             </rss>
             """);
    }
    
    [Fact]
    public async Task ShouldReturnNPostsIfRequested()
    {
        var request = Substitute.For<HttpRequest>();
        request.Scheme.Returns("http");
        request.Host.Returns(new HostString("localhost"));
        request.PathBase.Returns(PathString.FromUriComponent("/"));
        var httpContext = Substitute.For<HttpContext>();
        httpContext.Request.Returns(request);
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };
        var config = Options.Create(new ApplicationConfigurationBuilder()
            .WithBlogName("Test")
            .Build());

        var introduction = new IntroductionBuilder()
            .WithDescription("Description")
            .Build();
        var introductionConfig = Options.Create(introduction);
        var blogPost1 = new BlogPostBuilder()
            .WithTitle("1")
            .WithShortDescription("Short 1")
            .WithPreviewImageUrl("preview1")
            .WithUpdatedDate(new DateTime(2022, 5, 1, 0, 0, 0, DateTimeKind.Utc))
            .WithTags("C#", ".NET")
            .Build();
        var blogPost2 = new BlogPostBuilder()
            .WithTitle("2")
            .WithContent("**Content 2**")
            .WithPreviewImageUrl("preview2")
            .WithUpdatedDate(new DateTime(2022, 6, 1, 0, 0, 0, DateTimeKind.Utc))
            .Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        var cut = new RssFeedController(introductionConfig, config, Repository)
        {
            ControllerContext = controllerContext,
        };

        var xml = await cut.GetRssFeed(withContent: true, numberOfBlogPosts: 1) as FileContentResult;

        xml.ShouldNotBeNull();
        var content = Encoding.UTF8.GetString(xml.FileContents);
        content.ShouldMatch(
            $"""
            .*
            <rss version="2.0">
              <channel>
                <title>Test</title>
                <link>http://localhost/</link>
                <description>Description</description>
                <item>
                  <guid isPermaLink="false">{blogPost2.Id}</guid>
                  <link>http://localhost//blogPost/{blogPost2.Id}</link>
                  <title>2</title>
                  <pubDate>Wed, 01 Jun 2022 00:00:00.*</pubDate>
                  <description><!\[CDATA\[<p><strong>Content 2</strong></p>
            ]]></description>
                  <image>preview2</image>
                </item>
              </channel>
            </rss>
            """);
    }
    
    [Fact]
    public async Task ShouldRespectBlogPostsPerPage()
    {
        var request = Substitute.For<HttpRequest>();
        request.Scheme.Returns("http");
        request.Host.Returns(new HostString("localhost"));
        request.PathBase.Returns(PathString.FromUriComponent("/"));
        var httpContext = Substitute.For<HttpContext>();
        httpContext.Request.Returns(request);
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };
        var config = Options.Create(new ApplicationConfigurationBuilder()
            .WithBlogName("Test")
            .WithBlogPostsPerPage(1)
            .Build());

        var introduction = new IntroductionBuilder()
            .WithDescription("Description")
            .Build();
        var introductionConfig = Options.Create(introduction);
        var blogPost1 = new BlogPostBuilder()
            .WithTitle("1")
            .WithShortDescription("Short 1")
            .WithPreviewImageUrl("preview1")
            .WithUpdatedDate(new DateTime(2022, 5, 1, 0, 0, 0, DateTimeKind.Utc))
            .WithTags("C#", ".NET")
            .Build();
        var blogPost2 = new BlogPostBuilder()
            .WithTitle("2")
            .WithContent("**Content 2**")
            .WithPreviewImageUrl("preview2")
            .WithUpdatedDate(new DateTime(2022, 6, 1, 0, 0, 0, DateTimeKind.Utc))
            .Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        var cut = new RssFeedController(introductionConfig, config, Repository)
        {
            ControllerContext = controllerContext,
        };

        var xml = await cut.GetRssFeed(withContent: true) as FileContentResult;

        xml.ShouldNotBeNull();
        var content = Encoding.UTF8.GetString(xml.FileContents);
        content.ShouldMatch(
            $"""
             .*
             <rss version="2.0">
               <channel>
                 <title>Test</title>
                 <link>http://localhost/</link>
                 <description>Description</description>
                 <item>
                   <guid isPermaLink="false">{blogPost2.Id}</guid>
                   <link>http://localhost//blogPost/{blogPost2.Id}</link>
                   <title>2</title>
                   <pubDate>Wed, 01 Jun 2022 00:00:00.*</pubDate>
                   <description><!\[CDATA\[<p><strong>Content 2</strong></p>
             ]]></description>
                   <image>preview2</image>
                 </item>
               </channel>
             </rss>
             """);
    }
}