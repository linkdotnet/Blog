using System;
using System.Globalization;
using System.Linq;
using LinkDotNet.Blog.Domain.MarkdownImport;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Web.Features.MarkdownImport;

public sealed partial class MarkdownImportParser
{
    private const string Delimiter = "----------";
    private readonly ILogger<MarkdownImportParser> logger;

    public MarkdownImportParser(ILogger<MarkdownImportParser> logger)
    {
        this.logger = logger;
    }

    public MarkdownContent? Parse(string markdownText, string fileName)
    {
        ArgumentNullException.ThrowIfNull(markdownText);

        try
        {
            var sections = markdownText.Split(new[] { Delimiter }, StringSplitOptions.None);

            if (sections.Length < 4)
            {
                LogInvalidFormat(fileName, "Expected at least 3 sections separated by delimiter");
                return null;
            }

            var headerSection = sections[1].Trim();
            var shortDescriptionSection = sections[2].Trim();
            var contentSection = string.Join(Delimiter, sections.Skip(3)).Trim();

            var metadata = ParseMetadata(headerSection, fileName);
            if (metadata is null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(shortDescriptionSection))
            {
                LogInvalidFormat(fileName, "Short description section is empty");
                return null;
            }

            if (string.IsNullOrWhiteSpace(contentSection))
            {
                LogInvalidFormat(fileName, "Content section is empty");
                return null;
            }

            return new MarkdownContent(metadata, shortDescriptionSection, contentSection);
        }
        catch (Exception ex)
        {
            LogParseException(fileName, ex);
            return null;
        }
    }

    private MarkdownMetadata? ParseMetadata(string headerSection, string fileName)
    {
        var lines = headerSection.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var fields = lines
            .Select(line => line.Split(':', 2, StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0].ToUpperInvariant(), parts => parts[1], StringComparer.OrdinalIgnoreCase);

        if (!fields.TryGetValue("ID", out var id) || string.IsNullOrWhiteSpace(id))
        {
            LogMissingRequiredField(fileName, "id");
            return null;
        }

        if (!fields.TryGetValue("title", out var title) || string.IsNullOrWhiteSpace(title))
        {
            LogMissingRequiredField(fileName, "title");
            return null;
        }

        if (!fields.TryGetValue("image", out var image) || string.IsNullOrWhiteSpace(image))
        {
            LogMissingRequiredField(fileName, "image");
            return null;
        }

        if (!fields.TryGetValue("published", out var publishedStr) || 
            !bool.TryParse(publishedStr, out var published))
        {
            LogMissingRequiredField(fileName, "published");
            return null;
        }

        var tags = fields.TryGetValue("tags", out var tagsStr) && !string.IsNullOrWhiteSpace(tagsStr)
            ? tagsStr.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            : Array.Empty<string>();

        var fallbackImage = fields.TryGetValue("fallbackimage", out var fallback) && !string.IsNullOrWhiteSpace(fallback)
            ? fallback
            : null;

        DateTime? updatedDate = null;
        if (fields.TryGetValue("updateddate", out var updatedDateStr) && !string.IsNullOrWhiteSpace(updatedDateStr))
        {
            if (DateTime.TryParse(updatedDateStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var parsedDate))
            {
                updatedDate = parsedDate;
            }
            else
            {
                LogInvalidField(fileName, "updatedDate", updatedDateStr);
            }
        }

        var authorName = fields.TryGetValue("AUTHORNAME", out var author) && !string.IsNullOrWhiteSpace(author)
            ? author
            : null;

        return new MarkdownMetadata(id, title, image, published, tags, fallbackImage, updatedDate, authorName);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to parse markdown file '{FileName}': {Reason}")]
    private partial void LogInvalidFormat(string fileName, string reason);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Missing required field '{FieldName}' in file '{FileName}'")]
    private partial void LogMissingRequiredField(string fileName, string fieldName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Invalid value for field '{FieldName}' in file '{FileName}': {Value}")]
    private partial void LogInvalidField(string fileName, string fieldName, string value);

    [LoggerMessage(Level = LogLevel.Error, Message = "Exception parsing markdown file '{FileName}'")]
    private partial void LogParseException(string fileName, Exception ex);
}
