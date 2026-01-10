using LinkDotNet.Blog.Web.Features;
using NCronJob;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.RegistrationExtensions;

public static class BackgroundServiceRegistrationExtensions
{
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        });

        services.AddNCronJob(options =>
        {
            options
                .AddJob<BlogPostPublisher>(p => p.WithCronExpression("* * * * *"))
                .ExecuteWhen(s => s.RunJob<SimilarBlogPostJob>()
                    .OnlyIf((IOptions<ApplicationConfiguration> applicationConfiguration) => applicationConfiguration.Value.ShowSimilarPosts));

            options.AddJob<TransformBlogPostRecordsJob>(p => p.WithCronExpression("0/10 * * * *"));
            options.AddJob<SimilarBlogPostJob>(c => c
                .WithName(nameof(SimilarBlogPostJob))
                .OnlyIf((IOptions<ApplicationConfiguration> applicationConfiguration) => applicationConfiguration.Value.ShowSimilarPosts));
            
            // Run PopularTagsJob every hour to update tag suggestions cache
            options.AddJob<PopularTagsJob>(p => p.WithCronExpression("0 * * * *"));
        });
    }
}
