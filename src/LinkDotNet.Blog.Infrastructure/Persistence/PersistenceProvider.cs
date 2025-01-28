using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

public sealed class PersistenceProvider : Enumeration<PersistenceProvider>
{
    public static readonly PersistenceProvider SqlServer = new(nameof(SqlServer));
    public static readonly PersistenceProvider Sqlite = new(nameof(Sqlite));
    public static readonly PersistenceProvider RavenDb = new(nameof(RavenDb));
    public static readonly PersistenceProvider MySql = new(nameof(MySql));
    public static readonly PersistenceProvider MongoDB = new(nameof(MongoDB));
    public static readonly PersistenceProvider PostgreSql = new(nameof(PostgreSql));

    private PersistenceProvider(string key)
        : base(key)
    {
    }
}
