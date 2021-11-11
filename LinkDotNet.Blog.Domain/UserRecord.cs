using System;

namespace LinkDotNet.Blog.Domain;

public class UserRecord : Entity
{
    public int UserIdentifierHash { get; set; }

    public DateTime DateTimeUtcClicked { get; set; }

    public string UrlClicked { get; set; }
}
