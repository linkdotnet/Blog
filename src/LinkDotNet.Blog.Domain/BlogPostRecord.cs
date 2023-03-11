using System;

namespace LinkDotNet.Blog.Domain;

public class BlogPostRecord : Entity
{
    public string BlogPostId { get; set; }

    public DateOnly DateClicked { get; set; }

    public int Clicks { get; set; }
}
