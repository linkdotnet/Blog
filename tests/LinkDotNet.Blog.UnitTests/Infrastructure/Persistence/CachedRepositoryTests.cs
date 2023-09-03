using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace LinkDotNet.Blog.UnitTests.Infrastructure.Persistence;

public sealed class CachedRepositoryTests
{
    private readonly IRepository<BlogPost> repositoryMock;
    private readonly CachedRepository<BlogPost> sut;

    public CachedRepositoryTests()
    {
        repositoryMock = Substitute.For<IRepository<BlogPost>>();
        sut = new CachedRepository<BlogPost>(repositoryMock, new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task ShouldGetFromCacheWhenLoaded()
    {
        var blogPost = new BlogPostBuilder().Build();
        repositoryMock.GetByIdAsync("id").Returns(blogPost);
        var firstCall = await sut.GetByIdAsync("id");

        var secondCall = await sut.GetByIdAsync("id");

        firstCall.Should().Be(secondCall);
        firstCall.Should().Be(blogPost);
        await repositoryMock.Received(1).GetByIdAsync("id");
    }

    [Fact]
    public async Task ShouldNotCacheWhenParameterDifferent()
    {
        SetupRepository();
        await sut.GetAllAsync();
        await sut.GetAllAsync(p => p.IsPublished);
        await sut.GetAllAsync(p => p.IsPublished, p => p.Likes);
        await sut.GetAllAsync(
            p => p.IsPublished,
            p => p.Likes,
            false);
        await sut.GetAllAsync(
            p => p.IsPublished,
            p => p.Likes,
            false,
            2);
        await sut.GetAllAsync(
            p => p.IsPublished,
            p => p.Likes,
            false,
            2,
            30);

        await repositoryMock.Received(6).GetAllAsync(
                Arg.Any<Expression<Func<BlogPost, bool>>>(), 
                Arg.Any<Expression<Func<BlogPost, object>>>(), 
                Arg.Any<bool>(), 
                Arg.Any<int>(), 
                Arg.Any<int>());
    }

    [Fact]
    public async Task ShouldUpdateCacheOnStore()
    {
        var blogPost = new BlogPostBuilder().Build();
        repositoryMock.When(r => r.StoreAsync(blogPost))
            .Do(_ => blogPost.Id = "id");
        repositoryMock.GetByIdAsync("id").Returns(blogPost);
        var update = new BlogPostBuilder().WithTitle("new").Build();
        blogPost.Update(update);
        await sut.StoreAsync(blogPost);

        var latest = await sut.GetByIdAsync("id");

        latest.Title.Should().Be("new");
    }

    [Fact]
    public async Task ShouldDelete()
    {
        await sut.DeleteAsync("id");

        await repositoryMock.Received(1).DeleteAsync("id");
    }

    [Fact]
    public async Task ShouldGetFreshDataAfterDelete()
    {
        SetupRepository();
        await sut.GetAllAsync();
        await sut.DeleteAsync("some_id");

        await sut.GetAllAsync();

        await repositoryMock.Received(2).GetAllAsync(
                Arg.Any<Expression<Func<BlogPost, bool>>>(), 
                Arg.Any<Expression<Func<BlogPost, object>>>(), 
                Arg.Any<bool>(), 
                Arg.Any<int>(), 
                Arg.Any<int>());
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("some_id")]
    public async Task ShouldNotThrowExceptionWhenCallingStoreWithoutRetrievingKeyFirst(string id)
    {
        var blogPost = new BlogPostBuilder().Build();
        blogPost.Id = id;
        
        await sut.StoreAsync(blogPost);
        
        await repositoryMock.Received(1).StoreAsync(blogPost);
    }
    
    [Fact]
    public async Task ShouldNotThrowExceptionWhenCallingDeleteWithoutRetrievingKeyFirst()
    {
        await sut.DeleteAsync("some_id");
        
        await repositoryMock.Received(1).DeleteAsync("some_id");
    }

    private void SetupRepository()
    {
        var blogPost = new BlogPostBuilder().Build();

        repositoryMock.GetAllAsync(Arg.Any<Expression<Func<BlogPost, bool>>>(),
            Arg.Any<Expression<Func<BlogPost, object>>>(),
            Arg.Any<bool>(),
            Arg.Any<int>(),
            Arg.Any<int>()).Returns(new PagedList<BlogPost>(new[] { blogPost }, 1, 1));
    }
}