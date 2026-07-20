using System;

namespace LinkDotNet.Blog.Domain;

public sealed class Series : Entity
{
    public string Name { get; set; } = default!;
}
