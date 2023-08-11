using System;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests;

public static class RepositoryTestExtensions
{
    public static void RegisterRepositoryWithEmptyReturn<TEntity>(this IServiceCollection collection)
        where TEntity : Entity
    {
        var repositoryMock = Substitute.For<IRepository<TEntity>>();
        repositoryMock.GetAllAsync(
                Arg.Any<Expression<Func<TEntity, bool>>>(), 
                Arg.Any<Expression<Func<TEntity, object>>>(), 
                Arg.Any<bool>(), 
                Arg.Any<int>(), 
                Arg.Any<int>())
            .Returns(new PagedList<TEntity>(Array.Empty<TEntity>(), 1, 1));

        collection.AddScoped(_ => repositoryMock);
    }
}
