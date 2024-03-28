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

        services.AddNCronJob(p => p.TimerInterval = TimeSpan.FromSeconds(30));
        services.AddCronJob<BlogPostPublisher>(p => p.CronExpression = "* * * * *");
        services.AddCronJob<TransformBlogPostRecordsJob>(p => p.CronExpression = "0 * * * *");
    }
}
