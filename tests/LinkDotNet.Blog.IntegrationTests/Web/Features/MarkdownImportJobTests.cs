using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Domain.MarkdownImport;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.MarkdownImport;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCronJob;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Features;

public class MarkdownImportJobTests : SqlDatabaseTestBase<BlogPost>
{
    private readonly IMarkdownSourceProvider mockSourceProvider;
    private readonly MarkdownImportParser parser;
    private readonly ICacheInvalidator mockCacheInvalidator;
    private readonly IOptions<ApplicationConfiguration> appConfiguration;

    public MarkdownImportJobTests()
    {
        mockSourceProvider = Substitute.For<IMarkdownSourceProvider>();
        parser = new MarkdownImportParser(Substitute.For<ILogger<MarkdownImportParser>>());
        mockCacheInvalidator = Substitute.For<ICacheInvalidator>();
        appConfiguration = Options.Create(new ApplicationConfiguration
        {
            BlogName = "Test Blog",
            ConnectionString = "Data Source=:memory:",
            DatabaseName = "TestDb",
            MarkdownImport = new MarkdownImportConfiguration
            {
                Enabled = true,
                SourceType = "FlatDirectory",
                Url = "https://example.com"
            }
        });
    }

    [Fact]
    public async Task Should_Create_New_Post_From_Markdown()
    {
        // Arrange
        var markdownContent = """
            ----------
            id: test-post-1
            title: Test Blog Post
            image: https://example.com/image.jpg
            published: true
            tags: csharp, testing
            ----------
            This is a short description
            ----------
            # Content
            This is the main content of the blog post.
            """;

        var markdownFiles = new List<MarkdownFile>
        {
            new("test-post.md", markdownContent)
        };

        mockSourceProvider.GetMarkdownFilesAsync(Arg.Any<CancellationToken>())
            .Returns(markdownFiles);

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            appConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var posts = await Repository.GetAllAsync();
        posts.Count.ShouldBe(1);
        
        var post = posts[0];
        post.Title.ShouldBe("Test Blog Post");
        post.ShortDescription.ShouldBe("This is a short description");
        post.Content.ShouldContain("# Content");
        post.PreviewImageUrl.ShouldBe("https://example.com/image.jpg");
        post.IsPublished.ShouldBeTrue();
        post.Tags.ShouldContain("csharp");
        post.Tags.ShouldContain("testing");
        post.ExternalId.ShouldBe("test-post-1");

        await mockCacheInvalidator.Received(1).ClearCacheAsync();
    }

    [Fact]
    public async Task Should_Update_Existing_Post_By_ExternalId()
    {
        // Arrange
        var existingPost = BlogPost.Create(
            title: "Old Title",
            shortDescription: "Old description",
            content: "Old content",
            previewImageUrl: "https://example.com/old.jpg",
            isPublished: false,
            externalId: "test-post-1");

        await Repository.StoreAsync(existingPost);

        var markdownContent = """
            ----------
            id: test-post-1
            title: Updated Title
            image: https://example.com/new.jpg
            published: true
            tags: csharp, updated
            ----------
            Updated short description
            ----------
            # Updated Content
            This is the updated content.
            """;

        var markdownFiles = new List<MarkdownFile>
        {
            new("test-post.md", markdownContent)
        };

        mockSourceProvider.GetMarkdownFilesAsync(Arg.Any<CancellationToken>())
            .Returns(markdownFiles);

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            appConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var posts = await Repository.GetAllAsync();
        posts.Count.ShouldBe(1);
        
        var post = posts[0];
        post.Id.ShouldBe(existingPost.Id); // Should be the same post
        post.Title.ShouldBe("Updated Title");
        post.ShortDescription.ShouldBe("Updated short description");
        post.Content.ShouldContain("# Updated Content");
        post.PreviewImageUrl.ShouldBe("https://example.com/new.jpg");
        post.IsPublished.ShouldBeTrue();
        post.ExternalId.ShouldBe("test-post-1");

        await mockCacheInvalidator.Received(1).ClearCacheAsync();
    }

    [Fact]
    public async Task Should_Not_Run_When_Feature_Is_Disabled()
    {
        // Arrange
        var disabledAppConfiguration = Options.Create(new ApplicationConfiguration
        {
            BlogName = "Test Blog",
            ConnectionString = "Data Source=:memory:",
            DatabaseName = "TestDb",
            MarkdownImport = new MarkdownImportConfiguration
            {
                Enabled = false
            }
        });

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            disabledAppConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var posts = await Repository.GetAllAsync();
        posts.Count.ShouldBe(0);

        await mockSourceProvider.DidNotReceive().GetMarkdownFilesAsync(Arg.Any<CancellationToken>());
        await mockCacheInvalidator.DidNotReceive().ClearCacheAsync();
    }

    [Fact]
    public async Task Should_Not_Run_When_Configuration_Is_Null()
    {
        // Arrange
        var nullAppConfiguration = Options.Create(new ApplicationConfiguration
        {
            BlogName = "Test Blog",
            ConnectionString = "Data Source=:memory:",
            DatabaseName = "TestDb",
            MarkdownImport = null
        });

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            nullAppConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var posts = await Repository.GetAllAsync();
        posts.Count.ShouldBe(0);

        await mockSourceProvider.DidNotReceive().GetMarkdownFilesAsync(Arg.Any<CancellationToken>());
        await mockCacheInvalidator.DidNotReceive().ClearCacheAsync();
    }

    [Fact]
    public async Task Should_Handle_Invalid_Markdown_Gracefully()
    {
        // Arrange
        var invalidMarkdownContent = """
            This is not valid markdown format
            Missing delimiters
            """;

        var markdownFiles = new List<MarkdownFile>
        {
            new("invalid.md", invalidMarkdownContent)
        };

        mockSourceProvider.GetMarkdownFilesAsync(Arg.Any<CancellationToken>())
            .Returns(markdownFiles);

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            appConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var posts = await Repository.GetAllAsync();
        posts.Count.ShouldBe(0);

        // Cache should not be cleared when no posts are created or updated
        await mockCacheInvalidator.DidNotReceive().ClearCacheAsync();
    }

    [Fact]
    public async Task Should_Handle_Source_Provider_Exception()
    {
        // Arrange
        mockSourceProvider.GetMarkdownFilesAsync(Arg.Any<CancellationToken>())
            .Returns<IReadOnlyCollection<MarkdownFile>>(_ => throw new Exception("Network error"));

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            appConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var posts = await Repository.GetAllAsync();
        posts.Count.ShouldBe(0);

        await mockCacheInvalidator.DidNotReceive().ClearCacheAsync();
    }

    [Fact]
    public async Task Should_Handle_Parsing_Exception_For_Individual_File()
    {
        // Arrange
        var validMarkdownContent = """
            ----------
            id: valid-post
            title: Valid Post
            image: https://example.com/image.jpg
            published: true
            ----------
            Short description
            ----------
            Content
            """;

        var invalidMarkdownContent = """
            ----------
            id: invalid-post
            title: Missing required fields
            ----------
            Short description
            ----------
            Content
            """;

        var markdownFiles = new List<MarkdownFile>
        {
            new("valid.md", validMarkdownContent),
            new("invalid.md", invalidMarkdownContent)
        };

        mockSourceProvider.GetMarkdownFilesAsync(Arg.Any<CancellationToken>())
            .Returns(markdownFiles);

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            appConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var posts = await Repository.GetAllAsync();
        posts.Count.ShouldBe(1); // Only valid post should be created
        posts[0].ExternalId.ShouldBe("valid-post");

        // Cache should still be cleared because one post was created
        await mockCacheInvalidator.Received(1).ClearCacheAsync();
    }

    [Fact]
    public async Task Should_Import_Multiple_Posts()
    {
        // Arrange
        var markdown1 = """
            ----------
            id: post-1
            title: Post 1
            image: https://example.com/1.jpg
            published: true
            ----------
            Description 1
            ----------
            Content 1
            """;

        var markdown2 = """
            ----------
            id: post-2
            title: Post 2
            image: https://example.com/2.jpg
            published: false
            ----------
            Description 2
            ----------
            Content 2
            """;

        var markdownFiles = new List<MarkdownFile>
        {
            new("post1.md", markdown1),
            new("post2.md", markdown2)
        };

        mockSourceProvider.GetMarkdownFilesAsync(Arg.Any<CancellationToken>())
            .Returns(markdownFiles);

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            appConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        var posts = await Repository.GetAllAsync();
        posts.Count.ShouldBe(2);
        
        posts.ShouldContain(p => p.ExternalId == "post-1");
        posts.ShouldContain(p => p.ExternalId == "post-2");

        await mockCacheInvalidator.Received(1).ClearCacheAsync();
    }

    [Fact]
    public async Task Should_Not_Clear_Cache_When_No_Changes()
    {
        // Arrange
        mockSourceProvider.GetMarkdownFilesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<MarkdownFile>());

        var sut = new MarkdownImportJob(
            Repository,
            mockSourceProvider,
            parser,
            mockCacheInvalidator,
            appConfiguration,
            Substitute.For<ILogger<MarkdownImportJob>>());

        // Act
        await sut.RunAsync(Substitute.For<IJobExecutionContext>(), TestContext.Current.CancellationToken);

        // Assert
        await mockCacheInvalidator.DidNotReceive().ClearCacheAsync();
    }
}
