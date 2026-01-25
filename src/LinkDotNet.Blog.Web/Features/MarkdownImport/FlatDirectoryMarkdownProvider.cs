using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features.MarkdownImport;

public sealed partial class FlatDirectoryMarkdownProvider : IMarkdownSourceProvider
{
    private readonly HttpClient httpClient;
    private readonly ILogger<FlatDirectoryMarkdownProvider> logger;

    public FlatDirectoryMarkdownProvider(
        HttpClient httpClient,
        ILogger<FlatDirectoryMarkdownProvider> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<IReadOnlyCollection<MarkdownFile>> GetMarkdownFilesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var baseUri = httpClient.BaseAddress ?? throw new InvalidOperationException("BaseAddress not configured");
            LogFetchingFiles(baseUri.ToString());

            var directoryContent = await httpClient.GetStringAsync(baseUri, cancellationToken);
            var markdownFiles = ExtractMarkdownFileNames(directoryContent);

            LogFoundFiles(markdownFiles.Count);

            var files = new List<MarkdownFile>();
            foreach (var fileName in markdownFiles.OrderBy(f => f))
            {
                try
                {
                    var fileUri = new Uri(baseUri, fileName);
                    var content = await httpClient.GetStringAsync(fileUri, cancellationToken);
                    files.Add(new MarkdownFile(fileName, content));
                    LogFileDownloaded(fileName);
                }
                catch (Exception ex)
                {
                    LogFileDownloadFailed(fileName, ex);
                }
            }

            return files;
        }
        catch (Exception ex)
        {
            LogFetchFailed(httpClient.BaseAddress?.ToString() ?? "unknown", ex);
            return Array.Empty<MarkdownFile>();
        }
    }

    private static List<string> ExtractMarkdownFileNames(string htmlContent)
    {
        var regex = MarkdownLinkRegex();
        var matches = regex.Matches(htmlContent);
        return matches
            .Select(m => m.Groups[1].Value)
            .Where(f => f.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    [GeneratedRegex(@"href=""([^""]+\.md)""", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MarkdownLinkRegex();

    [LoggerMessage(Level = LogLevel.Information, Message = "Fetching markdown files from: {BaseUrl}")]
    private partial void LogFetchingFiles(string baseUrl);

    [LoggerMessage(Level = LogLevel.Information, Message = "Found {Count} markdown files")]
    private partial void LogFoundFiles(int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Downloaded file: {FileName}")]
    private partial void LogFileDownloaded(string fileName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to download file '{FileName}'")]
    private partial void LogFileDownloadFailed(string fileName, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to fetch markdown files from '{BaseUrl}'")]
    private partial void LogFetchFailed(string baseUrl, Exception ex);
}
