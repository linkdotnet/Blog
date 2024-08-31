using System;

namespace LinkDotNet.Blog.Domain;

public sealed class UserRecord : Entity
{
    public DateOnly DateClicked { get; init; }

    public required string UrlClicked { get; init; }
}
