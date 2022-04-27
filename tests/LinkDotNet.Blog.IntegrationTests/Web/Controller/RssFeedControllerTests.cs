using System;
using System.Text;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Controller;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinkDotNet.Blog.IntegrationTests.Web.Controller;

public class RssFeedControllerTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldCreateRssFeed()
    {
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
        var blogPost2 = new BlogPostBuilder()
            .WithTitle("2")
            .WithShortDescription("Short 2")
            .WithUpdatedDate(new DateTime(2022, 6, 1))
            .Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        var cut = new RssFeedController(config, Repository)
        {
            ControllerContext = controllerContext,
        };

        var xml = await cut.GetRssFeed() as FileContentResult;

        xml.Should().NotBeNull();
        var content = Encoding.UTF8.GetString(xml.FileContents);
        content.Should().NotBeNull();
    }
}