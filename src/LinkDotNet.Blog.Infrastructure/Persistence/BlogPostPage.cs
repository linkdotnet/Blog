using System.Collections.Generic;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed record BlogPostPage(
    BlogPost BlogPost,
    IReadOnlyList<ShortCode> ShortCodes,
    IReadOnlyList<BlogPostSummary> SimilarBlogPosts);
