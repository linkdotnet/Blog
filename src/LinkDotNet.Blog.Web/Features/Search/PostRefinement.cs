using System;
using System.Collections.Generic;
using System.Linq;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.Services.Tags;

namespace LinkDotNet.Blog.Web.Features.Search;

public static class PostRefinement
{
    public static IReadOnlyList<TagCount> CountTags(IEnumerable<BlogPost> posts, string? excludedTag = null)
    {
        return posts
            .SelectMany(p => p.Tags.Distinct())
            .Where(tag => !string.Equals(tag, excludedTag, StringComparison.Ordinal))
            .GroupBy(tag => tag)
            .Select(group => new TagCount(group.Key, group.Count()))
            .OrderByDescending(t => t.Count)
            .ThenBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    public static IReadOnlyList<BlogPost> Apply(
        IEnumerable<BlogPost> posts,
        IReadOnlySet<string> requiredTags,
        string text,
        PostSortOrder sortOrder)
    {
        var trimmedText = text.Trim();

        var matches = posts
            .Where(p => requiredTags.All(p.Tags.Contains))
            .Where(p => trimmedText.Length == 0 || Matches(p, trimmedText));

        return sortOrder switch
        {
            PostSortOrder.Oldest => matches.OrderBy(p => p.UpdatedDate).ToList(),
            PostSortOrder.ShortestRead => matches.OrderBy(p => p.ReadingTimeInMinutes).ThenByDescending(p => p.UpdatedDate).ToList(),
            _ => matches.OrderByDescending(p => p.UpdatedDate).ToList(),
        };
    }

    private static bool Matches(BlogPost post, string text)
    {
        return post.Title.Contains(text, StringComparison.CurrentCultureIgnoreCase)
               || post.ShortDescription.Contains(text, StringComparison.CurrentCultureIgnoreCase)
               || post.Tags.Any(t => t.Contains(text, StringComparison.CurrentCultureIgnoreCase));
    }
}
