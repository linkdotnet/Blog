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

        await scope.ServiceProvider.GetRequiredService<IRepository<ProfileInformationEntry>>().StoreBulkAsync(GenerateDummyProfileEntries());
        await scope.ServiceProvider.GetRequiredService<IRepository<Skill>>().StoreBulkAsync(GenerateDummySkills());
        await scope.ServiceProvider.GetRequiredService<IRepository<Talk>>().StoreBulkAsync(GenerateDummyTalks());
    }

    private static List<ProfileInformationEntry> GenerateDummyProfileEntries() =>
    [
        ProfileInformationEntry.Create("Passionate software engineer with broad experience and deep .NET knowledge.", 1000),
        ProfileInformationEntry.Create("Birthday: 17.05.1991", 2000),
        ProfileInformationEntry.Create("Speaks German and English", 3000),
        ProfileInformationEntry.Create("[Microsoft MVP](https://mvp.microsoft.com)", 4000),
        ProfileInformationEntry.Create("Hobbies: Playing piano and sports (calisthenics)", 5000),
    ];

    private static List<Skill> GenerateDummySkills() =>
    [
        Skill.Create("C#", "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg", "Backend", "Expert"),
        Skill.Create("EF Core", null, "Backend", "Expert"),
        Skill.Create("gRPC", null, "Backend", "Proficient"),
        Skill.Create("Python", "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/python/python-original.svg", "Backend", "Familiar"),
        Skill.Create("Blazor", "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/blazor/blazor-original.svg", "Frontend", "Expert"),
        Skill.Create("TypeScript", "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/typescript/typescript-original.svg", "Frontend", "Proficient"),
        Skill.Create("React", "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/react/react-original.svg", "Frontend", "Familiar"),
        Skill.Create("SQL Server", null, "Database", "Expert"),
        Skill.Create("RavenDB", null, "Database", "Proficient"),
        Skill.Create("GitHub Actions", "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/githubactions/githubactions-original.svg", "DevOps", "Expert"),
        Skill.Create("Domain-Driven Design", null, "Practices", "Expert"),
        Skill.Create("Scrum", null, "Project Management", "Proficient"),
    ];

    private static List<Talk> GenerateDummyTalks()
    {
        var today = DateTime.Today;
        return
        [
            Talk.Create("Async State Machines, Lowered", "NDC London", "Following a single await down to the generated state machine.", today.AddDays(30)),
            Talk.Create("SIMD in .NET", ".NET Switzerland", "[Slides](https://example.com/slides.pdf)", today.AddMonths(-3)),
            Talk.Create("Speedrun into Massive Data in C#", "NDC Oslo", "Parsing a billion rows on a laptop. [Slides](https://example.com/slides.pdf) · [Recording](https://example.com/video)", today.AddYears(-1)),
            Talk.Create("Clean Architecture with Blazor", ".NET User Group Zurich", "Structuring a Blazor app so the UI stays a detail.", today.AddYears(-3)),
        ];
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static List<BlogPost> GenerateDummyBlogPosts(int count)
    {
        const string loremIpsum =
            """
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.

            Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.

            > [!NOTE]
            > Callouts render as Bootstrap alerts. This one checks spacing and contrast in both themes.

            ## Getting Set Up

            Before diving in, make sure you have the following installed:

            - The latest .NET SDK
            - An editor with C# support
            - A terminal you are comfortable with

            ### Project Structure

            Run `dotnet new console` and you get a minimal project. The interesting parts live in `Program.cs`.

            ![Landscape photo](https://picsum.photos/seed/content/1200/600)

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

            ### A Longer Snippet

            ```csharp
            app.MapGet("/weatherforecast", async (IWeatherService service, CancellationToken token) =>
            {
                var forecasts = await service.GetForecastsAsync(days: 5, cancellationToken: token);
                return forecasts.Count == 0 ? Results.NoContent() : Results.Ok(forecasts);
            });
            ```

            ## Comparing Options

            | Approach | Startup | Memory | Notes |
            |---|---|---|---|
            | JIT | Slower | Higher | Most flexible |
            | ReadyToRun | Faster | Medium | Larger binaries |
            | Native AOT | Fastest | Lowest | No runtime reflection |

            > Simplicity is prerequisite for reliability.

            ## More Content

            Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo.

            Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt.
            """;

        var titles = new[]
        {
            "Getting Started with C# 12",
            "Array, List, Collection, Set, ReadOnlyList - what? A comprehensive and exhaustive list of collection-like types",
            "Understanding Blazor Components",
            "How to write your own cron Job scheduler in ASP.NET Core (like Quartz, Hangfire, ...)",
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
            "Performance",
            "Security",
            "OpenTelemetry",
            "Minimal APIs",
            ".NET Aspire",
            "Source Generators",
            "Kubernetes",
            "GitHub Actions",
        };

        var blogPosts = new List<BlogPost>();

        for (var i = 0; i < count; i++)
        {
            var title = i < titles.Length ? titles[i] : $"Blog Post {i + 1}";
            var shortDescription = (i % 3) switch
            {
                0 => $"A short look at {title}.",
                1 => $"This is a dummy blog post about {title}. It contains valuable information for developers.",
                _ => $"This is a dummy blog post about {title}. It walks through the motivation, shows a minimal example, compares the trade-offs against the alternatives and ends with a few practical tips you can apply in your own code base today.",
            };

            var selectedTags = tags
                .OrderBy(_ => Random.Shared.Next())
                .Take(Random.Shared.Next(1, 9))
                .ToArray();

            var blogPost = BlogPost.Create(
                title,
                shortDescription,
                loremIpsum,
                $"https://picsum.photos/seed/blog-{i}/1200/630",
                true,
                DateTime.UtcNow.AddDays(-i * 23),
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
