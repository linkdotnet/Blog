using System;
using System.Diagnostics;

namespace LinkDotNet.Blog.Domain;

[DebuggerDisplay("{Content} with sort order {SortOrder}")]
public sealed class ProfileInformationEntry : Entity
{
    private ProfileInformationEntry()
    {
    }

    public string Content { get; private init; }

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
