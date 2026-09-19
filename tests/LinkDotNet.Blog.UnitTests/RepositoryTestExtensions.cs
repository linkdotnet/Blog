using System;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
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
            .Returns(new PagedList<TEntity>([], 0, 1, 1));

        collection.AddScoped(_ => repositoryMock);

        if (typeof(TEntity) == typeof(BlogPost))
        {
            collection.RegisterBlogPostRepository((IRepository<BlogPost>)repositoryMock);
        }
        else if (typeof(TEntity) == typeof(ShortCode))
        {
            collection.RegisterShortCodeRepository((IRepository<ShortCode>)repositoryMock);
        }
        else if (typeof(TEntity) == typeof(BlogPostTemplate))
        {
            collection.RegisterBlogPostTemplateRepository((IRepository<BlogPostTemplate>)repositoryMock);
        }
        else if (typeof(TEntity) == typeof(BrokenLink))
        {
            collection.RegisterBrokenLinkRepository((IRepository<BrokenLink>)repositoryMock);
        }
        else if (typeof(TEntity) == typeof(SimilarBlogPost))
        {
            collection.RegisterSimilarBlogPostRepository((IRepository<SimilarBlogPost>)repositoryMock);
        }
        else if (typeof(TEntity) == typeof(ProfileInformationEntry))
        {
            collection.RegisterProfileInformationEntryRepository((IRepository<ProfileInformationEntry>)repositoryMock);
        }
        else if (typeof(TEntity) == typeof(Skill))
        {
            collection.RegisterSkillRepository((IRepository<Skill>)repositoryMock);
        }
        else if (typeof(TEntity) == typeof(Talk))
        {
            collection.RegisterTalkRepository((IRepository<Talk>)repositoryMock);
        }
    }
}
