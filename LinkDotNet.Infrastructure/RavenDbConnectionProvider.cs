using Raven.Client.Documents;

namespace LinkDotNet.Infrastructure
{
    public static class RavenDbConnectionProvider
    {
        public static IDocumentStore Create(string url, string databaseName)
        {
            var documentStore = new DocumentStore {Urls = new[] {url}, Database = databaseName};
            documentStore.Initialize();
            return documentStore;
        }
    }
}