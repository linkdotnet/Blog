using System.Text.RegularExpressions;
using CommandLine;
using LinkDotNet.Blog.CriticalCSS;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using Microsoft.Extensions.DependencyInjection;

return await Parser.Default.ParseArguments<CommandLineOptions>(args)
    .MapResult(
        async opts => await RunWithOptions(opts),
        _ => Task.FromResult(1));

static async Task<int> RunWithOptions(CommandLineOptions options)
{
    if (options.Help)
    {
        ShowHelp();
        return 0;
    }

    if (options.InstallPlaywright)
    {
        InstallPlaywrightDependencies();
    }

    var css = await ExtractCriticalCss();

    if (options.OutputMode is null)
    {
        throw new InvalidOperationException("The OutputMode is required.");
    }

    if (options.OutputMode.Equals("console", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine(css);
    }
    else if (options.OutputMode.Equals("file", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(options.FilePath))
    {
        OutputToFile(css, options.FilePath);
    }
    else if (options.OutputMode.Equals("layout", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(options.FilePath))
    {
        OutputToLayout(css, options.FilePath);
    }
    else
    {
        await Console.Error.WriteLineAsync(
            "Invalid output mode or missing file path. Use --help for usage information.");
        return 1;
    }

    return 0;
}

static void InstallPlaywrightDependencies() => Microsoft.Playwright.Program.Main(["install"]);

static async Task<string> ExtractCriticalCss()
{
    var factory = new PlaywrightWebApplicationFactory();
    using var httpClient = factory.CreateClient();
    var blogPost = await SaveBlogPostAsync(factory.Services);

    List<string> urls =
    [
        $"{factory.ServerAddress}",
        $"{factory.ServerAddress}blogPost/{blogPost.Id}",
    ];

    var criticalCss = await CriticalCssGenerator.GenerateAsync(urls);
    await factory.DisposeAsync();
    return criticalCss;

    static async Task<BlogPost> SaveBlogPostAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository<BlogPost>>();
        var blogPost = new BlogPostBuilder()
            .WithTitle("Test")
            .WithShortDescription("**Test**")
            .WithContent("""
                         > Quote
                         ```csharp
                         public async internal void Test()
                         {
                             await Task.Delay(100);
                         }
                         ```
                         """)
            .Build();
        await repository.StoreAsync(blogPost);
        return blogPost;
    }
}

static void OutputToFile(string css, string? outputPath)
{
    ArgumentException.ThrowIfNullOrEmpty(css);
    ArgumentException.ThrowIfNullOrEmpty(outputPath);

    File.WriteAllText(outputPath, css);
}

static void OutputToLayout(string css, string? layoutPath)
{
    ArgumentException.ThrowIfNullOrEmpty(css);
    ArgumentException.ThrowIfNullOrEmpty(layoutPath);

    var layoutContent = File.ReadAllText(layoutPath);
    const string styleTagPattern = "<style[^>]*>.*?</style>";
    const string headEndTag = "</head>";


    layoutContent = Regex.IsMatch(layoutContent, styleTagPattern, RegexOptions.Singleline)
        ? Regex.Replace(layoutContent, styleTagPattern, css, RegexOptions.Singleline)
        : layoutContent.Replace(headEndTag, $"{css}\n    {headEndTag}", StringComparison.OrdinalIgnoreCase);

    File.WriteAllText(layoutPath, layoutContent);
}

static void ShowHelp()
{
    Console.WriteLine("Critical CSS Extractor");
    Console.WriteLine("Usage: criticalcss [options]");
    Console.WriteLine("\nOptions:");
    Console.WriteLine("  -i, --install-playwright  Install Playwright dependencies");
    Console.WriteLine("  -o, --output <mode>      Output mode: console, file, or layout (required)");
    Console.WriteLine("  -p, --path <path>        File path when using file or layout output mode");
    Console.WriteLine("  -h, --help               Show this help message");
    Console.WriteLine("\nExamples:");
    Console.WriteLine("  criticalcss --output console");
    Console.WriteLine("  criticalcss --output file --path styles.css");
    Console.WriteLine("  criticalcss --output layout --path _Layout.cshtml");
    Console.WriteLine("  criticalcss --install-playwright --output console");
}
