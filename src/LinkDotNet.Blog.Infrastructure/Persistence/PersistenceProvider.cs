using LinkDotNet.Blog.Generators;

namespace LinkDotNet.Blog.Infrastructure.Persistence;

[Enumeration("SqlServer", "Sqlite", "RavenDb", "MySql", "MongoDB", "PostgreSql")]
public sealed partial record PersistenceProvider;

