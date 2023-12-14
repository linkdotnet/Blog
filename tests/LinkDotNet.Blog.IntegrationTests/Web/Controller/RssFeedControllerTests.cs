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
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.IntegrationTests.Web.Controller;

public class RssFeedControllerTests
{
    [Fact]
    public async Task ShouldCreateRssFeed()
    {
        var repository = new Repository<BlogPost>();
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
        var config = Options.Create<ApplicationConfiguration>(new ApplicationConfiguration
        {
            BlogName = "Test",
            Introduction = new Introduction
            {
                Description = "Description",
            },
        });
        var blogPost1 = new BlogPostBuilder()
            .WithTitle("1")
            .WithShortDescription("Short 1")
            .WithPreviewImageUrl("preview1")
            .WithUpdatedDate(new DateTime(2022, 5, 1))
            .WithTags("C#", ".NET")
            .Build();
        blogPost1.Id = "1";
        var blogPost2 = new BlogPostBuilder()
            .WithTitle("2")
            .WithShortDescription("**Short 2**")
            .WithPreviewImageUrl("preview2")
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
        content.Should().Match(
@"*<rss version=""2.0"">
  <channel>
    <title>Test</title>
    <link>http://localhost/</link>
    <description>Description</description>
    <item>
      <guid isPermaLink=""false"">2</guid>
      <link>http://localhost//blogPost/2</link>
      <title>2</title>
      <description>Short 2</description>
      <pubDate>Wed, 01 Jun 2022 00:00:00*</pubDate>
      <image>preview2</image>
    </item>
    <item>
      <guid isPermaLink=""false"">1</guid>
      <link>http://localhost//blogPost/1</link>
      <category>C#</category>
      <category>.NET</category>
      <title>1</title>
      <description>Short 1</description>
      <pubDate>Sun, 01 May 2022 00:00:00*</pubDate>
      <image>preview1</image>
    </item>
  </channel>
</rss>");
    }
}