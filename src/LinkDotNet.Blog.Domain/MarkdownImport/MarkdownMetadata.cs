using System;
using System.Collections.Generic;

namespace LinkDotNet.Blog.Domain.MarkdownImport;

public sealed record MarkdownMetadata(
    string Id,
    string Title,
    string Image,
    bool Published,
    IReadOnlyCollection<string> Tags,
    string? FallbackImage,
    DateTime? UpdatedDate,
    string? AuthorName);
