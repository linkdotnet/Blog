using System;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed class BlogPostRevisionConflictException : Exception
{
    public BlogPostRevisionConflictException()
    {
    }

    public BlogPostRevisionConflictException(string message)
        : base(message)
    {
    }

    public BlogPostRevisionConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
