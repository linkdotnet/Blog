using System;
using System.Linq;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Search;

public static class BlogPostSearch
{
    public static Expression<Func<BlogPost, bool>> ByTerm(string term)
        => b => b.IsPublished && (b.Title.Contains(term) || b.Tags.Any(t => t == term));

    public static Expression<Func<BlogPost, bool>> ByTag(string tag)
        => b => b.IsPublished && b.Tags.Any(t => t == tag);
}
