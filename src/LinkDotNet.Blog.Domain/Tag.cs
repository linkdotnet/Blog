using System;

namespace LinkDotNet.Blog.Domain;

public sealed class Tag
{
    private Tag()
    {
    }

    public string Id { get; private set; }

    public string Content { get; private set; }

    public static Tag Create(string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        return new Tag
        {
            Content = content.Trim(),
        };
    }
}
