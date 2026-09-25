using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Infrastructure.Persistence.RavenDb;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Testcontainers.RavenDb;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.RavenDb;

internal static class RavenDbTestContainer
{
    private static readonly TestContainer Container = TestContainer.Create("RavenDB", () => new RavenDbBuilder("ravendb/ravendb:7.2-latest").Build(), c => c.GetConnectionString());

    public static async Task<IDocumentStore> CreateDocumentStoreAsync()
    {
        var url = await Container.GetConnectionStringAsync();
        var databaseName = "test-" + Guid.NewGuid().ToString("N");
        using (var serverStore = new DocumentStore { Urls = [url] }.Initialize())
        {
            await serverStore.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(databaseName)));
        }

        return RavenDbConnectionProvider.Create(url, databaseName);
    }
}
