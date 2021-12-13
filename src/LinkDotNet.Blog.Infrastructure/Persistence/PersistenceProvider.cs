using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public class PersistenceProvider : Enumeration<PersistenceProvider>
{
    public static readonly PersistenceProvider SqlServer = new(nameof(SqlServer));
    public static readonly PersistenceProvider SqliteServer = new(nameof(SqliteServer));
    public static readonly PersistenceProvider RavenDb = new(nameof(RavenDb));
    public static readonly PersistenceProvider InMemory = new(nameof(InMemory));

    protected PersistenceProvider(string key)
        : base(key)
    {
    }
}
