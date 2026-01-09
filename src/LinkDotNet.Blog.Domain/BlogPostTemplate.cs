using System;

namespace LinkDotNet.Blog.Domain;

public sealed class BlogPostTemplate : Entity
{
    public string Name { get; private set; } = default!;

    public string Title { get; private set; } = default!;

    public string ShortDescription { get; private set; } = default!;

    public string Content { get; private set; } = default!;

    public static BlogPostTemplate Create(string name, string title, string shortDescription, string content)
    {
        return new BlogPostTemplate
        {
            Name = name,
            Title = title,
            ShortDescription = shortDescription,
            Content = content
        };
    }

    public void Update(string name, string title, string shortDescription, string content)
    {
        Name = name;
        Title = title;
        ShortDescription = shortDescription;
        Content = content;
    }
}
