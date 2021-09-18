using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared.Admin.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages.Admin.Dashboard
{
    public class VisitCountPerPageTests : SqlDatabaseTestBase<BlogPost>
    {
        [Fact]
        public async Task ShouldShowCounts()
        {
            var blogPost = new BlogPostBuilder().WithTitle("I was clicked").WithLikes(2).Build();
            await Repository.StoreAsync(blogPost);
            using var ctx = new TestContext();
            ctx.Services.AddScoped<IRepository<BlogPost>>(_ => Repository);
            var visits = new List<KeyValuePair<string, int>>();
            visits.Add(new KeyValuePair<string, int>($"blogPost/{blogPost.Id}", 5));
            var pageVisitCounts = visits.OrderByDescending(s => s.Value);

            var cut = ctx.RenderComponent<VisitCountPerPage>(p => p.Add(
                s => s.PageVisitCount, pageVisitCounts));

            cut.WaitForState(() => cut.FindAll("td").Any());
            var elements = cut.FindAll("td").ToList();
            elements.Count.Should().Be(3);
            var titleData = elements[0].ChildNodes.Single() as IHtmlAnchorElement;
            titleData.Should().NotBeNull();
            titleData.InnerHtml.Should().Be(blogPost.Title);
            titleData.Href.Should().Contain($"blogPost/{blogPost.Id}");
            elements[1].InnerHtml.Should().Be("5");
            elements[2].InnerHtml.Should().Be("2");
        }
    }
}