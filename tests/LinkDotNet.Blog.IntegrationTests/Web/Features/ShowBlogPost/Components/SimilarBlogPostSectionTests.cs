using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features.ShowBlogPost.Components;

public class SimilarBlogPostSectionTests : SqlDatabaseTestBase<BlogPost>
{
    [Fact]
    public async Task ShouldShowSimilarBlogPosts()
    {
        var blogPost1 = new BlogPostBuilder().WithTitle("Title 1").Build();
        var blogPost2 = new BlogPostBuilder().WithTitle("Title 2").Build();
        var blogPost3 = new BlogPostBuilder().WithTitle("Title 3").Build();
        await Repository.StoreAsync(blogPost1);
        await Repository.StoreAsync(blogPost2);
        await Repository.StoreAsync(blogPost3);
        var similarBlogPost1 = new SimilarBlogPost
        {
            Id = blogPost1.Id,
            SimilarBlogPostIds = [blogPost2.Id, blogPost3.Id]
        };
        await DbContext.SimilarBlogPosts.AddAsync(similarBlogPost1, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        await using var context = new BunitContext();
        context.Services.AddScoped<IRepository<SimilarBlogPost>>(_ =>
            new Repository<SimilarBlogPost>(DbContextFactory, Substitute.For<ILogger<Repository<SimilarBlogPost>>>()));
        context.Services.AddScoped(_ => Repository);
        
        var cut = context.Render<SimilarBlogPostSection>(p => p.Add(s => s.BlogPost, blogPost1));

        var elements = cut.WaitForElements(".card-title");
        elements.Count.ShouldBe(2);
        elements.ShouldContain(p => p.TextContent == "Title 2");
        elements.ShouldContain(p => p.TextContent == "Title 3");
    }
}
