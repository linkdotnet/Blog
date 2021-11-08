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

namespace LinkDotNet.Blog.IntegrationTests.Web.Pages.Admin.Dashboard
{
    public sealed class VisitCountPerPageTests : SqlDatabaseTestBase<BlogPost>
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
}