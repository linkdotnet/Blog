﻿namespace LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;

public sealed record GiscusConfiguration
{
    public const string GiscusConfigurationSection = "Giscus";

    public string Repository { get; init; }

    public string RepositoryId { get; init; }

    public string Category { get; init; }

    public string CategoryId { get; init; }
}
