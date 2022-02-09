using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;
using Microsoft.Extensions.DependencyInjection;

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
    public async Task ShouldFilterByDate()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("1").WithLikes(2).Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("2").WithLikes(2).Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        var clicked1 = new UserRecord
        { UrlClicked = $"blogPost/{blogPost1.Id}", DateTimeUtcClicked = new DateTime(2020, 1, 1) };
        var clicked2 = new UserRecord
        { UrlClicked = $"blogPost/{blogPost1.Id}", DateTimeUtcClicked = DateTime.MinValue };
        var clicked3 = new UserRecord
        { UrlClicked = $"blogPost/{blogPost2.Id}", DateTimeUtcClicked = DateTime.MinValue };
        var clicked4 = new UserRecord
        { UrlClicked = $"blogPost/{blogPost1.Id}", DateTimeUtcClicked = new DateTime(2021, 1, 1) };
        await DbContext.UserRecords.AddRangeAsync(new[] { clicked1, clicked2, clicked3, clicked4 });
        await DbContext.SaveChangesAsync();
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => DbContext);
        var cut = ctx.RenderComponent<VisitCountPerPage>();

        cut.FindComponent<DateRangeSelector>().Find("#startDate").Change(new DateTime(2019, 1, 1));
        cut.FindComponent<DateRangeSelector>().Find("#endDate").Change(new DateTime(2020, 12, 31));

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