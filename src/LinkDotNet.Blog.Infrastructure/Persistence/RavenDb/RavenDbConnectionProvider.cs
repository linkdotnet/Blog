using Raven.Client.Documents;

namespace LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;

public static class RavenDbConnectionProvider
{
    public static IDocumentStore Create(string url, string databaseName)
    {
        var documentStore = new DocumentStore
        {
            Urls = [url],
            Database = databaseName,
            Conventions = { IdentityPartsSeparator = '-' },
        };
#pragma warning disable IDISP004 // Handled by the DI Container
        documentStore.Initialize();
#pragma warning restore
        return documentStore;
    }
}
