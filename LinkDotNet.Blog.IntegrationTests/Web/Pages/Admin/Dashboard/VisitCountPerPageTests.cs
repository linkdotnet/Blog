using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared.Admin.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages.Admin.Dashboard;

public class VisitCountPerPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldShowCounts()
    {
        var blogPost = new BlogPostBuilder().WithTitle("I was clicked").WithLikes(2).Build();
        await Repository.StoreAsync(blogPost);
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => DbContext);
        await SaveBlogPostArticleClicked(blogPost.Id, 10);

        var cut = ctx.RenderComponent<VisitCountPerPage>();

        cut.WaitForState(() => cut.FindAll("td").Any());
        var elements = cut.FindAll("td").ToList();
        elements.Count.Should().Be(3);
        var titleData = elements[0].ChildNodes.Single() as IHtmlAnchorElement;
        titleData.Should().NotBeNull();
        titleData.InnerHtml.Should().Be(blogPost.Title);
        titleData.Href.Should().Contain($"blogPost/{blogPost.Id}");
        elements[1].InnerHtml.Should().Be("10");
        elements[2].InnerHtml.Should().Be("2");
    }

    [Fact]
    public async Task ShouldFilterStartDate()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("1").WithLikes(2).Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("2").WithLikes(2).Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        var urlClicked1New = new UserRecord
        { UrlClicked = $"blogPost/{blogPost1.Id}", DateTimeUtcClicked = DateTime.UtcNow };
        var urlClicked1Old = new UserRecord
        { UrlClicked = $"blogPost/{blogPost1.Id}", DateTimeUtcClicked = DateTime.MinValue };
        var urlClicked2 = new UserRecord
        { UrlClicked = $"blogPost/{blogPost2.Id}", DateTimeUtcClicked = DateTime.MinValue };
        await DbContext.UserRecords.AddRangeAsync(new[] { urlClicked1New, urlClicked1Old, urlClicked2 });
        await DbContext.SaveChangesAsync();
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => DbContext);
        var cut = ctx.RenderComponent<VisitCountPerPage>();

        cut.FindComponent<DateRangeSelector>().Find("select").Change(DateTime.UtcNow.Date);

        cut.WaitForState(() => cut.FindAll("td").Any());
        var elements = cut.FindAll("td").ToList();
        elements.Count.Should().Be(3);
        var titleData = elements[0].ChildNodes.Single() as IHtmlAnchorElement;
        titleData.Should().NotBeNull();
        titleData.InnerHtml.Should().Be(blogPost1.Title);
        titleData.Href.Should().Contain($"blogPost/{blogPost1.Id}");
        elements[1].InnerHtml.Should().Be("1");
    }

    private async Task SaveBlogPostArticleClicked(string blogPostId, int count)
    {
        var urlClicked = $"blogPost/{blogPostId}";
        for (var i = 0; i < count; i++)
        {
            var data = new UserRecord
            {
                UrlClicked = urlClicked,
            };
            await DbContext.UserRecords.AddAsync(data);
        }

        await DbContext.SaveChangesAsync();
    }
}
