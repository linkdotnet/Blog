using Azure.Storage.Blobs.Models;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services.Tags;

public sealed class TagQueryService(IRepository<BlogPost> blogPostRepository) : ITagQueryService
{
    public async Task<IReadOnlyList<TagCount>> GetAllOrderedByUsageAsync()
    {
        var posts = await blogPostRepository.GetAllAsync();

        var tagCounts = posts
            // Flatten the collection of tag lists into a single sequence.
            .SelectMany(p => p.Tags ?? Enumerable.Empty<string>())

            // Defensive guard against invalid tag values.
            .Where(tag => !string.IsNullOrEmpty(tag))

            .GroupBy(tag => tag.Trim())

            // Transform each group into a TagCount DTO.
            // group.Key = tag name
            // group.Count() = number of occurrences
            .Select(group => new TagCount(
                group.Key,
                group.Count()))

            // Sort descending by usage count (most popular first).
            .OrderByDescending(tc => tc.Count)
            .ThenBy(tc => tc.Name)
            .ToList();

        return tagCounts;
    }
}
