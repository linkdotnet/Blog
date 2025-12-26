using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.DummyData;

internal sealed class DummyDataSeeder : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly DummyDataOptions options;

    public DummyDataSeeder(IServiceProvider serviceProvider, DummyDataOptions options)
    {
        this.serviceProvider = serviceProvider;
        this.options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<BlogPost>>();

        var blogPosts = GenerateDummyBlogPosts(options.NumberOfBlogPosts);
        await repository.StoreBulkAsync(blogPosts);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static List<BlogPost> GenerateDummyBlogPosts(int count)
    {
        const string loremIpsum =
            """
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.

            Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.

            ## Code Example

            ```csharp
            public class Example
            {
                public void HelloWorld()
                {
                    Console.WriteLine("Hello, World!");
                }
            }
            ```

            ## More Content

            Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.
            """;

        var titles = new[]
        {
            "Getting Started with C# 12",
            "Understanding Blazor Components",
            "ASP.NET Core Best Practices",
            "Mastering Entity Framework Core",
            "Introduction to LINQ",
            "Building RESTful APIs",
            "Dependency Injection in .NET",
            "Working with SignalR",
            "Testing in .NET with xUnit",
            "Advanced C# Features",
            "Docker and .NET Applications",
            "Microservices Architecture",
            "Azure DevOps CI/CD",
            "Authentication and Authorization",
            "Performance Optimization Tips",
            "Modern Web Development",
            "Clean Code Principles",
            "Design Patterns in C#",
            "Asynchronous Programming",
            "Working with Databases",
        };

        var tags = new[]
        {
            "CSharp",
            "Blazor",
            "ASP.NET",
            "EntityFramework",
            "LINQ",
            "WebAPI",
            "Testing",
            "Docker",
            "Azure",
            "Architecture",
        };

        var blogPosts = new List<BlogPost>();

        for (var i = 0; i < count; i++)
        {
            var title = i < titles.Length ? titles[i] : $"Blog Post {i + 1}";
            var shortDescription = $"This is a dummy blog post about {title}. It contains valuable information for developers.";

            var selectedTags = tags
                .OrderBy(_ => Random.Shared.Next())
                .Take(Random.Shared.Next(1, 4))
                .ToArray();

            var blogPost = BlogPost.Create(
                title,
                shortDescription,
                loremIpsum,
                "https://via.placeholder.com/800x400",
                true,
                DateTime.UtcNow.AddDays(-i),
                null,
                selectedTags,
                null,
                "Dummy Author");

            blogPost.Likes = Random.Shared.Next(0, 10);
            blogPosts.Add(blogPost);
        }

        return blogPosts;
    }
}
