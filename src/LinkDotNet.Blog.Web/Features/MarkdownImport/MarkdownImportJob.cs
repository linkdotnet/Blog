using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCronJob;

namespace LinkDotNet.Blog.Web.Features.MarkdownImport;

public sealed partial class MarkdownImportJob : IJob
{
    private readonly IRepository<BlogPost> blogPostRepository;
    private readonly IMarkdownSourceProvider sourceProvider;
    private readonly MarkdownImportParser parser;
    private readonly ICacheInvalidator cacheInvalidator;
    private readonly ILogger<MarkdownImportJob> logger;
    private readonly MarkdownImportConfiguration? configuration;

    public MarkdownImportJob(
        IRepository<BlogPost> blogPostRepository,
        IMarkdownSourceProvider sourceProvider,
        MarkdownImportParser parser,
        ICacheInvalidator cacheInvalidator,
        IOptions<ApplicationConfiguration> appConfiguration,
        ILogger<MarkdownImportJob> logger)
    {
        ArgumentNullException.ThrowIfNull(appConfiguration);

        this.blogPostRepository = blogPostRepository;
        this.sourceProvider = sourceProvider;
        this.parser = parser;
        this.cacheInvalidator = cacheInvalidator;
        this.logger = logger;
        this.configuration = appConfiguration.Value.MarkdownImport;
    }

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (configuration is null || !configuration.Enabled)
        {
            LogJobDisabled();
            return;
        }

        LogJobStarting();

        var importedCount = 0;
        var updatedCount = 0;
        var errorCount = 0;

        try
        {
            var markdownFiles = await sourceProvider.GetMarkdownFilesAsync(token);
            LogFilesRetrieved(markdownFiles.Count);

            foreach (var file in markdownFiles)
            {
                try
                {
                    var parsedContent = parser.Parse(file.Content, file.FileName);
                    if (parsedContent is null)
                    {
                        errorCount++;
                        continue;
                    }

                    var existingPost = await blogPostRepository.GetAllAsync(
                        filter: bp => bp.ExternalId == parsedContent.Metadata.Id);

                    if (existingPost.Count > 0)
                    {
                        await UpdateExistingPostAsync(existingPost[0], parsedContent);
                        updatedCount++;
                        LogPostUpdated(file.FileName, parsedContent.Metadata.Id);
                    }
                    else
                    {
                        await CreateNewPostAsync(parsedContent);
                        importedCount++;
                        LogPostCreated(file.FileName, parsedContent.Metadata.Id);
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    LogFileProcessingFailed(file.FileName, ex);
                }
            }

            if (importedCount > 0 || updatedCount > 0)
            {
                await cacheInvalidator.ClearCacheAsync();
            }

            LogJobCompleted(importedCount, updatedCount, errorCount);
        }
        catch (Exception ex)
        {
            LogJobFailed(ex);
        }
    }

    private async Task UpdateExistingPostAsync(BlogPost existingPost, Domain.MarkdownImport.MarkdownContent content)
    {
        var updatedPost = BlogPost.Create(
            title: content.Metadata.Title,
            shortDescription: content.ShortDescription,
            content: content.Content,
            previewImageUrl: content.Metadata.Image,
            isPublished: content.Metadata.Published,
            updatedDate: content.Metadata.UpdatedDate,
            tags: content.Metadata.Tags,
            previewImageUrlFallback: content.Metadata.FallbackImage,
            authorName: content.Metadata.AuthorName,
            externalId: content.Metadata.Id);

        existingPost.Update(updatedPost);
        await blogPostRepository.StoreAsync(existingPost);
    }

    private async Task CreateNewPostAsync(Domain.MarkdownImport.MarkdownContent content)
    {
        var newPost = BlogPost.Create(
            title: content.Metadata.Title,
            shortDescription: content.ShortDescription,
            content: content.Content,
            previewImageUrl: content.Metadata.Image,
            isPublished: content.Metadata.Published,
            updatedDate: content.Metadata.UpdatedDate,
            tags: content.Metadata.Tags,
            previewImageUrlFallback: content.Metadata.FallbackImage,
            authorName: content.Metadata.AuthorName,
            externalId: content.Metadata.Id);

        await blogPostRepository.StoreAsync(newPost);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Markdown import job is disabled")]
    private partial void LogJobDisabled();

    [LoggerMessage(Level = LogLevel.Information, Message = "Markdown import job starting")]
    private partial void LogJobStarting();

    [LoggerMessage(Level = LogLevel.Information, Message = "Retrieved {Count} markdown files from source")]
    private partial void LogFilesRetrieved(int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Created new blog post from file '{FileName}' with ExternalId '{ExternalId}'")]
    private partial void LogPostCreated(string fileName, string externalId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Updated existing blog post from file '{FileName}' with ExternalId '{ExternalId}'")]
    private partial void LogPostUpdated(string fileName, string externalId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to process file '{FileName}'")]
    private partial void LogFileProcessingFailed(string fileName, Exception ex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Markdown import job completed: {ImportedCount} created, {UpdatedCount} updated, {ErrorCount} errors")]
    private partial void LogJobCompleted(int importedCount, int updatedCount, int errorCount);

    [LoggerMessage(Level = LogLevel.Error, Message = "Markdown import job failed")]
    private partial void LogJobFailed(Exception ex);
}
