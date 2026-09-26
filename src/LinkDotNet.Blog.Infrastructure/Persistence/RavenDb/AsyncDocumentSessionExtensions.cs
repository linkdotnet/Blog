using Raven.Client;
using Raven.Client.Documents.Session;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

internal static class AsyncDocumentSessionExtensions
{
    // Raven ids are global across collections, so e.g. a version id would otherwise load as a BlogPost.
    public static bool IsStoredAs<TEntity>(this IAsyncDocumentSession session, TEntity entity)
        where TEntity : notnull
    {
        var collection = session.Advanced.GetMetadataFor(entity)[Constants.Documents.Metadata.Collection] as string;
        return collection == session.Advanced.DocumentStore.Conventions.GetCollectionName(typeof(TEntity));
    }
}
