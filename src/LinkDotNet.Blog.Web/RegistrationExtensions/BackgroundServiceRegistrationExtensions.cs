using System;
using LinkDotNet.Blog.Web.Features;
using LinkDotNet.NCronJob;
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

        services.AddNCronJob();
        services.AddCronJob<BlogPostPublisher>(p => p.WithCronExpression("* * * * *"));
        services.AddCronJob<TransformBlogPostRecordsJob>(p => p.WithCronExpression("0/10 * * * *"));
    }
}
