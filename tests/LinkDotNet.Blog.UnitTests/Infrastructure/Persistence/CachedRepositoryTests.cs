﻿using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace LinkDotNet.Blog.UnitTests.Infrastructure.Persistence;

public sealed class CachedRepositoryTests : IDisposable
{
    private readonly Mock<IRepository<BlogPost>> repositoryMock;
    private readonly CachedRepository<BlogPost> sut;

    public CachedRepositoryTests()
    {
        repositoryMock = new Mock<IRepository<BlogPost>>();
        sut = new CachedRepository<BlogPost>(repositoryMock.Object, new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task ShouldGetFromCacheWhenLoaded()
    {
        var blogPost = new BlogPostBuilder().Build();
        repositoryMock.Setup(r => r.GetByIdAsync("id")).ReturnsAsync(blogPost);
        var firstCall = await sut.GetByIdAsync("id");

        var secondCall = await sut.GetByIdAsync("id");

        firstCall.Should().Be(secondCall);
        firstCall.Should().Be(blogPost);
        repositoryMock.Verify(r => r.GetByIdAsync("id"), Times.Once);
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

        repositoryMock.Verify(
            r => r.GetAllAsync(
                It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<Expression<Func<BlogPost, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()),
            Times.Exactly(6));
    }

    [Fact]
    public async Task ShouldUpdateCacheOnStore()
    {
        var blogPost = new BlogPostBuilder().Build();
        repositoryMock.Setup(r => r.StoreAsync(blogPost))
            .Callback(() => blogPost.Id = "id");
        repositoryMock.Setup(r => r.GetByIdAsync("id")).ReturnsAsync(blogPost);
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

        repositoryMock.Verify(r => r.DeleteAsync("id"), Times.Once);
    }

    [Fact]
    public async Task ShouldGetFreshDataAfterDelete()
    {
        SetupRepository();
        await sut.GetAllAsync();
        await sut.DeleteAsync("some_id");

        await sut.GetAllAsync();

        repositoryMock.Verify(
            r => r.GetAllAsync(
            It.IsAny<Expression<Func<BlogPost, bool>>>(),
            It.IsAny<Expression<Func<BlogPost, object>>>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<int>()),
            Times.Exactly(2));
    }

    public void Dispose()
    {
        sut.Dispose();
    }

    private void SetupRepository()
    {
        var blogPost = new BlogPostBuilder().Build();
        repositoryMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<Expression<Func<BlogPost, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PaginatedList<BlogPost>(new[] { blogPost }, 1, 1));
    }
}
