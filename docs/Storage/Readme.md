## Storage Provider

The blog currently supports six storage providers:

-   RavenDb - As the name suggests for RavenDb. RavenDb automatically creates all the documents, if a database name is provided.
-   MongoDB - Based on the official MongoDB driver. The database and collection are automatically created.
-   Sqlite - Based on EF Core, it can be easily adapted for other Sql Dialects. The tables are automatically created.
-   SqlServer - Based on EF Core, it can be easily adapted for other Sql Dialects. The tables are automatically created.
-   MySql - Based on EF Core - also supports MariaDB.
-   PostgreSql - Based on EF Core.

The checked-in development configuration uses `Sqlite` with a shared in-memory database. Its data is lost when the process stops, which is useful for testing. For persistent SQLite storage, set the connection string to a file path in `appsettings.json`:

```json
{
	"PersistenceProvider": "Sqlite",
	"ConnectionString": "Data Source=blog.db",
```

Note the ConnectionString format of SQL Server needs to be consistent:

```
"ConnectionString": "Data Source=sql;Initial Catalog=master;User ID=sa;Password=<YOURPASSWORD>;TrustServerCertificate=True;MultiSubnetFailover=True"
```

For MySql use the following:

```
"PersistenceProvider": "MySql"
"ConnectionString": "Server=YOURSERVER;User ID=YOURUSERID;Password=YOURPASSWORD;Database=YOURDATABASE"
```

For PostgreSql use the following:

```
"PersistenceProvider": "PostgreSql"
"ConnectionString": "Server=YOURSERVER;User ID=YOURUSERID;Password=YOURPASSWORD;Database=YOURDATABASE"
```

## Entity Framework Migrations

For the SQL providers (`SqlServer`, `Sqlite`, `MySql`, `PostgreSql`), use Entity Framework Core Migrations to update an existing database schema. The full documentation can be found under [*"Entity Framework Core tools reference"*](https://learn.microsoft.com/en-us/ef/core/cli/dotnet). For example:

```bash
dotnet ef database update --project src/LinkDotNet.Blog.Infrastructure --startup-project src/LinkDotNet.Blog.Web --connection "<ConnectionString>"
```

The `--connection` parameter is optional; without it, EF Core reads the connection string from application configuration. You can also generate a SQL script to review and execute against your database:

```bash
dotnet ef migrations script --project src/LinkDotNet.Blog.Infrastructure --startup-project src/LinkDotNet.Blog.Web  
```

Here is the full documentation: [*"Applying Migrations"*](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying).

On startup, the application calls `Database.EnsureCreated()`. This creates a schema only when the database has no tables; it does not upgrade an existing schema or apply EF Core migrations. Existing installations must apply the migrations (or an equivalent SQL script) when a release requires a database change. See [MIGRATION.md](../../MIGRATION.md) for release-specific steps.

## Considerations
For most people a Sqlite database might be the best choice between convienence and ease of setup. As it runs "in-process" there are no additional dependencies or setup required (and therefore no additional cost). As the blog tries to cache many things, the load onto the database is not that big (performance considerations). The advantages of a "real" database like SqlServer or MySql are more in the realm of backups, replication, and other enterprise features (which are not needed often times for a simple blog).
