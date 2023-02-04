using System;

namespace LinkDotNet.Blog.Domain;

public class UserRecord : Entity
{
    public int UserIdentifierHash { get; set; }

    public DateOnly DateClicked { get; set; }

    public string UrlClicked { get; set; }
}
