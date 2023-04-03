using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public class PersistenceProvider : Enumeration<PersistenceProvider>
{
    public static readonly PersistenceProvider SqlServer = new(nameof(SqlServer));
    public static readonly PersistenceProvider Sqlite = new(nameof(Sqlite));
    public static readonly PersistenceProvider RavenDb = new(nameof(RavenDb));
    public static readonly PersistenceProvider InMemory = new(nameof(InMemory));
    public static readonly PersistenceProvider MySql = new(nameof(MySql));

    protected PersistenceProvider(string key)
        : base(key)
    {
    }
}
