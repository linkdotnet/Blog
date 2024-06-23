using LinkDotNet.Blog.Web.Features;
using NCronJob;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                .ExecuteWhen(s => s.RunJob<SimilarBlogPostJob>());

            options.AddJob<TransformBlogPostRecordsJob>(p => p.WithCronExpression("0/10 * * * *"));
        });
    }
}
