using System;

namespace LinkDotNet.Blog.Domain;

public class Tag
{
    private Tag()
    {
    }

    public string Id { get; private set; }

    public string Content { get; private set; }

    public static Tag Create(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentNullException(nameof(content));
        }

        return new Tag
        {
            Content = content.Trim(),
        };
    }
}
