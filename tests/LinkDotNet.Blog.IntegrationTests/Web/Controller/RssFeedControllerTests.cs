using System;
using System.Text;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.InMemory;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinkDotNet.Blog.IntegrationTests.Web.Controller;

public class RssFeedControllerTests
{
    [Fact]
    public async Task ShouldCreateRssFeed()
    {
        var repository = new Repository<BlogPost>();
        var request = new Mock<HttpRequest>();
        request.Setup(x => x.Scheme).Returns("http");
        request.Setup(x => x.Host).Returns(new HostString("localhost"));
        request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/"));
        var httpContext = Mock.Of<HttpContext>(_ => _.Request == request.Object);
        var controllerContext = new ControllerContext
        {
            HttpContext = httpContext,
        };
        var config = new AppConfiguration
        {
            BlogName = "Test",
            Introduction = new Introduction
            {
                Description = "Description",
            },
        };
        var blogPost1 = new BlogPostBuilder()
            .WithTitle("1")
            .WithShortDescription("Short 1")
            .WithUpdatedDate(new DateTime(2022, 5, 1))
            .Build();
        blogPost1.Id = "1";
        var blogPost2 = new BlogPostBuilder()
            .WithTitle("2")
            .WithShortDescription("Short 2")
            .WithUpdatedDate(new DateTime(2022, 6, 1))
            .Build();
        blogPost2.Id = "2";
        await repository.StoreAsync(blogPost1);
        await repository.StoreAsync(blogPost2);
        var cut = new RssFeedController(config, repository)
        {
            ControllerContext = controllerContext,
        };

        var xml = await cut.GetRssFeed() as FileContentResult;

        xml.Should().NotBeNull();
        var content = Encoding.UTF8.GetString(xml.FileContents);
        content.Should().Contain("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<rss\r\n  version=\"2.0\">\r\n  <channel>\r\n    <title>Test</title>\r\n    <link>http://localhost/</link>\r\n    <description>Description</description>\r\n    <item>\r\n      <guid\r\n        isPermaLink=\"false\">2</guid>\r\n      <link>http://localhost//blogPost/2</link>\r\n      <title>2</title>\r\n      <description>Short 2</description>\r\n      <pubDate>Wed, 01 Jun 2022 00:00:00 +0200</pubDate>\r\n    </item>\r\n    <item>\r\n      <guid\r\n        isPermaLink=\"false\">1</guid>\r\n      <link>http://localhost//blogPost/1</link>\r\n      <title>1</title>\r\n      <description>Short 1</description>\r\n      <pubDate>Sun, 01 May 2022 00:00:00 +0200</pubDate>\r\n    </item>\r\n  </channel>\r\n</rss>");
    }
}