using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.TestUtilities;

public static class RepositoryRegistrationExtensions
{
    public static IServiceCollection RegisterBlogPostRepository(this IServiceCollection services, IRepository<BlogPost> repository)
    {
        services.AddScoped(_ => repository);
        services.AddScoped<IBlogPostRepository>(_ => new BlogPostRepository(repository));
        return services;
    }

    public static IServiceCollection RegisterShortCodeRepository(this IServiceCollection services, IRepository<ShortCode> repository)
    {
        services.AddScoped(_ => repository);
        services.AddScoped<IShortCodeRepository>(_ => new ShortCodeRepository(repository));
        return services;
    }

    public static IServiceCollection RegisterBlogPostTemplateRepository(this IServiceCollection services, IRepository<BlogPostTemplate> repository)
    {
        services.AddScoped(_ => repository);
        services.AddScoped<IBlogPostTemplateRepository>(_ => new BlogPostTemplateRepository(repository));
        return services;
    }

    public static IServiceCollection RegisterBrokenLinkRepository(this IServiceCollection services, IRepository<BrokenLink> repository)
    {
        services.AddScoped(_ => repository);
        services.AddScoped<IBrokenLinkRepository>(_ => new BrokenLinkRepository(repository));
        return services;
    }

    public static IServiceCollection RegisterSimilarBlogPostRepository(this IServiceCollection services, IRepository<SimilarBlogPost> repository)
    {
        services.AddScoped(_ => repository);
        services.AddScoped<ISimilarBlogPostRepository>(_ => new SimilarBlogPostRepository(repository));
        return services;
    }

    public static IServiceCollection RegisterProfileInformationEntryRepository(this IServiceCollection services, IRepository<ProfileInformationEntry> repository)
    {
        services.AddScoped(_ => repository);
        services.AddScoped<IAboutMeRepository, AboutMeRepository>();
        return services;
    }

    public static IServiceCollection RegisterSkillRepository(this IServiceCollection services, IRepository<Skill> repository)
    {
        services.AddScoped(_ => repository);
        services.AddScoped<IAboutMeRepository, AboutMeRepository>();
        return services;
    }

    public static IServiceCollection RegisterTalkRepository(this IServiceCollection services, IRepository<Talk> repository)
    {
        services.AddScoped(_ => repository);
        services.AddScoped<IAboutMeRepository, AboutMeRepository>();
        return services;
    }
}
