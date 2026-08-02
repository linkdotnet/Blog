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
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        repositoryMock.GetAllAsync().ReturnsForAnyArgs(PagedList<BlogPost>.Empty);
        Services.AddScoped(_ => repositoryMock);

        var cut = Render<SearchByTagPage>(p => p.Add(s => s.Tag, "C%23"));

        cut.FindComponent<OgData>().Instance.Robots.ShouldBe("noindex, follow");
    }
}
