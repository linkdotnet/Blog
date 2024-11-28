## Storage Provider

Currently, there are 5 Storage-Provider:

-   RavenDb - As the name suggests for RavenDb. RavenDb automatically creates all the documents, if a database name is provided.
-   MongoDB - Based on the official MongoDB driver. The database and collection are automatically created.
-   Sqlite - Based on EF Core, it can be easily adapted for other Sql Dialects. The tables are automatically created.
-   SqlServer - Based on EF Core, it can be easily adapted for other Sql Dialects. The tables are automatically created.
-   MySql - Based on EF Core - also supports MariaDB.

The default (when you clone the repository) is the `Sqlite` option with an in-memory database.
That means every time you restart the service, all posts and related objects are gone. This is useful for testing. 
If you want to persist the data with Sqlite, you can change the `appsettings.json` file to:

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

## Entity Framework Migrations

For the SQL providers (`SqlServer`, `Sqlite`, `MySql`), you can use Entity Framework Core Migrations to create and manage the database schema. The whole documentation can be found under [*"Entity Framework Core tools reference"*](https://learn.microsoft.com/en-us/ef/core/cli/dotnet). The short version is that you can use the following steps:

```bash
dotnet ef database update --project src/LinkDotNet.Blog.Infrastructure --startup-project src/LinkDotNet.Blog.Web --connection "<ConnectionString>"
```

The `--connection` parameter is optional - if you don't specify it, it will try to grab it from your `appsettings.json` file.
The other options is to create a sql script you can execute against your database:

```bash
dotnet ef migrations script --project src/LinkDotNet.Blog.Infrastructure --startup-project src/LinkDotNet.Blog.Web  
```

Here is the full documentation: [*"Applying Migrations"*](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying).

Alternatively, the blog calls `Database.EnsureCreated()` on startup, which creates the database schema if it does not exist. So you are not forced to use migrations.

## Considerations
For most people a Sqlite database might be the best choice between convienence and ease of setup. As it runs "in-process" there are no additional dependencies or setup required (and therefore no additional cost). As the blog tries to cache many things, the load onto the database is not that big (performance considerations). The advantages of a "real" database like SqlServer or MySql are more in the realm of backups, replication, and other enterprise features (which are not needed often times for a simple blog).