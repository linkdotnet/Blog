using System;
using System.Diagnostics;

namespace LinkDotNet.Blog.Domain;

[DebuggerDisplay("{Content} with sort order {SortOrder}")]
public sealed class ProfileInformationEntry : Entity
{
    public string Content { get; private init; } = default!;

    public int SortOrder { get; set; }

    public static ProfileInformationEntry Create(string key, int sortOrder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return new ProfileInformationEntry
        {
            Content = key.Trim(),
            SortOrder = sortOrder,
        };
    }
}
