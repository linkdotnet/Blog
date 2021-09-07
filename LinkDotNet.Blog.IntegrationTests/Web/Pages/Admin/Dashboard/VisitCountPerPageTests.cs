using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared.Admin.Dashboard;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages.Admin.Dashboard
{
    public class VisitCountPerPageTests : SqlDatabaseTestBase<BlogPost>
    {
        [Fact]
        public async Task ShouldShowCounts()
        {
            var blogPost = new BlogPostBuilder().WithTitle("I was clicked").Build();
            await Repository.StoreAsync(blogPost);
            using var ctx = new TestContext();
            ctx.Services.AddScoped<IRepository<BlogPost>>(_ => Repository);
            var visits = new List<KeyValuePair<string, int>>();
            visits.Add(new KeyValuePair<string, int>($"blogPost/{blogPost.Id}", 5));
            var pageVisitCounts = visits.OrderByDescending(s => s.Value);

            var cut = ctx.RenderComponent<VisitCountPerPage>(p => p.Add(
                s => s.PageVisitCount, pageVisitCounts));

            cut.WaitForState(() => cut.FindAll("td").Any());
            var elements = cut.FindAll("td").Select(t => t.InnerHtml).ToList();
            elements.Should().Contain("I was clicked");
            elements.Should().Contain("5");
        }
    }
}