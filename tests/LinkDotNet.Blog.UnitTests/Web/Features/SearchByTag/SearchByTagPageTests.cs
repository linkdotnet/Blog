using System.Threading.Tasks;
using System.Collections.Generic;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.SearchByTag;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.SearchByTag;

public class SearchByTagPageTests : BunitContext
{
    [Fact]
    public void ShouldNotIndexTagPages()
    {
        // Tag pages are thin listing pages that search engines refuse to index anyway.
        // Declaring noindex turns a repeated crawl-time judgement into an explicit instruction.
        var listQuery = Substitute.For<IBlogPostListQuery>();
        listQuery.GetPublishedByTagAsync(Arg.Any<string>()).Returns(new ValueTask<IReadOnlyList<BlogPostSummary>>([]));
        Services.AddScoped(_ => listQuery);

        var cut = Render<SearchByTagPage>(p => p.Add(s => s.Tag, "C%23"));

        cut.FindComponent<OgData>().Instance.Robots.ShouldBe("noindex, follow");
    }
}
