using System;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using X.PagedList;

namespace LinkDotNet.Blog.UnitTests;

public static class RepositoryTestExtensions
{
    public static void RegisterRepositoryWithEmptyReturn<TEntity>(this IServiceCollection collection)
        where TEntity : Entity
    {
        var repositoryMock = new Mock<IRepository<TEntity>>();
        repositoryMock
            .Setup(s => s.GetAllAsync(
                It.IsAny<Expression<Func<TEntity, bool>>>(),
                It.IsAny<Expression<Func<TEntity, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PagedList<TEntity>(Array.Empty<TEntity>(), 1, 1));

        collection.AddScoped(_ => repositoryMock.Object);
    }
}