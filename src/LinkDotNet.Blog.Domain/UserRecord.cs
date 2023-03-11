using System;

namespace LinkDotNet.Blog.Domain;

public sealed class UserRecord : Entity
{
    public DateOnly DateClicked { get; set; }

    public string UrlClicked { get; set; }
}
