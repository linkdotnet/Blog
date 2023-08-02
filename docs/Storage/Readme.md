## Storage Provider

Currently, there are 5 Storage-Provider:

-   InMemory - Basically a list holding your data (per request). If the User hits a hard reload, the data is gone.
-   RavenDb - As the name suggests for RavenDb. RavenDb automatically creates all the documents, if a database name is provided.
-   Sqlite - Based on EF Core, it can be easily adapted for other Sql Dialects. The tables are automatically created.
-   SqlServer - Based on EF Core, it can be easily adapted for other Sql Dialects. The tables are automatically created.
-   MySql - Based on EF Core - also supports MariaDB.

The default (when you clone the repository) is the `InMemory` option. That means every time you restart the service, all posts and related objects are gone.

Note the ConnectionString format of SQL Server needs to be consistent:

```
"ConnectionString": "Data Source=sql;Initial Catalog=master;User ID=sa;Password=<YOURPASSWORD>;TrustServerCertificate=True;MultiSubnetFailover=True"
```

For MySql use the following:

```
"PersistenceProvider": "MySql"
"ConnectionString": "Server=YOURSERVER;User ID=YOURUSERID;Password=YOURPASSWORD;Database=YOURDATABASE"
```
