using System;

namespace LinkDotNet.Blog.Domain;

public class BlogPostRecord : Entity
{
    public required string BlogPostId { get; init; }

    public DateOnly DateClicked { get; init; }

    public int Clicks { get; init; }
}
