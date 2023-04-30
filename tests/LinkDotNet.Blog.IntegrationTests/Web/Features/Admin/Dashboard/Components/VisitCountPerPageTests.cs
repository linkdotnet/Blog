using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharpWrappers;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.Admin.Dashboard.Components;

public class VisitCountPerPageTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldShowCounts()
    {
        var blogPost = new BlogPostBuilder().WithTitle("I was clicked").WithLikes(2).Build();
        await Repository.StoreAsync(blogPost);
        using var ctx = new TestContext();
        RegisterRepositories(ctx);
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
        { UrlClicked = $"blogPost/{blogPost1.Id}", DateClicked = new DateOnly(2020, 1, 1) };
        var clicked2 = new UserRecord
        { UrlClicked = $"blogPost/{blogPost1.Id}", DateClicked = DateOnly.MinValue };
        var clicked3 = new UserRecord
        { UrlClicked = $"blogPost/{blogPost2.Id}", DateClicked = DateOnly.MinValue };
        var clicked4 = new UserRecord
        { UrlClicked = $"blogPost/{blogPost1.Id}", DateClicked = new DateOnly(2021, 1, 1) };
        await DbContext.UserRecords.AddRangeAsync(clicked1, clicked2, clicked3, clicked4);
        await DbContext.SaveChangesAsync();
        using var ctx = new TestContext();
        ctx.ComponentFactories.Add<DateRangeSelector, FilterStubComponent>();
        RegisterRepositories(ctx);
        var cut = ctx.RenderComponent<VisitCountPerPage>();
        var filter = new Filter { StartDate = new DateOnly(2019, 1, 1), EndDate = new DateOnly(2020, 12, 31) };

        await cut.InvokeAsync(() => cut.FindComponent<FilterStubComponent>().Instance.FireFilterChanged(filter));

        cut.WaitForState(() => cut.FindAll("td").Count == 3);
        var elements = cut.FindAll("td").ToList();
        elements.Count.Should().Be(3);
        var titleData = elements[0].ChildNodes.Single() as IHtmlAnchorElement;
        titleData.Should().NotBeNull();
        titleData.InnerHtml.Should().Be(blogPost1.Title);
        titleData.Href.Should().Contain($"blogPost/{blogPost1.Id}");
        cut.WaitForAssertion(() => elements[1].InnerHtml.Should().Be("1"));
    }

    [Fact]
    public async Task ShouldShowTotalClickCount()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("1").WithLikes(2).Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("2").WithLikes(2).Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        var clicked1 = new UserRecord
            { UrlClicked = $"blogPost/{blogPost1.Id}", DateClicked = new DateOnly(2020, 1, 1) };
        var clicked2 = new UserRecord
            { UrlClicked = $"blogPost/{blogPost1.Id}", DateClicked = DateOnly.MinValue };
        var clicked3 = new UserRecord
            { UrlClicked = $"blogPost/{blogPost2.Id}", DateClicked = DateOnly.MinValue };
        var clicked4 = new UserRecord
            { UrlClicked = $"blogPost/{blogPost1.Id}", DateClicked = new DateOnly(2021, 1, 1) };
        await DbContext.UserRecords.AddRangeAsync(clicked1, clicked2, clicked3, clicked4);
        await DbContext.SaveChangesAsync();
        using var ctx = new TestContext();
        RegisterRepositories(ctx);

        var cut = ctx.RenderComponent<VisitCountPerPage>();

        cut.WaitForState(() => cut.FindAll("td").Any());
        cut.Find("#total-clicks").Unwrap().TextContent.Should().Be("4 clicks in total");
    }

    private void RegisterRepositories(TestContextBase ctx)
    {
        ctx.Services.AddScoped<IRepository<BlogPost>>(_ => new Repository<BlogPost>(DbContextFactory));
        ctx.Services.AddScoped<IRepository<UserRecord>>(_ => new Repository<UserRecord>(DbContextFactory));
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

    private sealed class FilterStubComponent : ComponentBase
    {
        [Parameter]
        public EventCallback<Filter> FilterChanged { get; set; }

        public void FireFilterChanged(Filter filter) => FilterChanged.InvokeAsync(filter);
    }
}
