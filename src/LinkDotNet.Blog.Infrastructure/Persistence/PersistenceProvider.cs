using LinkDotNet.Enumeration;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

[Enumeration("SqlServer", "Sqlite", "RavenDb", "MySql", "MongoDB", "PostgreSql")]
public sealed partial record PersistenceProvider
{
    public bool IsSql() =>
        Match(
            onSqlServer: true,
            onSqlite: true,
            onRavenDb: false,
            onMySql: true,
            onMongoDB: false,
            onPostgreSql: true);

    public bool IsMongoDB() => this == MongoDB;
    public bool IsRavenDb() => this == RavenDb;
}

