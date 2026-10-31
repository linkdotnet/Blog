## Host the blog in Docker containers

### Server configuration

The repository includes a `docker-compose.yml` example. Adjust the configuration, certificate paths, and database connection string for your deployment. The web container listens on ports 80 (HTTP) and 443 (HTTPS):

```yml
ports:
  - "80:80"
  - "443:443"
environment:
  - ASPNETCORE_URLS=http://+:80;https://+:443
  - ASPNETCORE_HTTPS_PORT=443
  - ASPNETCORE_Kestrel__Certificates__Default__Password=<certificate-password>
  - ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx
  - ASPNETCORE_ENVIRONMENT=Production
volumes:
  - ./src/LinkDotNet.Blog.Web/appsettings.json:/app/appsettings.json
  - /root/.aspnet/DataProtection-Keys:/root/.aspnet/DataProtection-Keys
  - ./aspnetapp.pfx:/app/aspnetapp.pfx
```

Set the connection string in `appsettings.json` to match the selected provider. For SQLite, the database path in the connection string must match the container mount path. Keep certificates and passwords out of source control.

Start the services with `docker compose up -d`. If TLS is terminated by a reverse proxy instead of Kestrel, remove the HTTPS URL and certificate settings and configure the proxy and forwarded headers for your deployment.

The sample Compose file includes SQL Server. If you use it, set `MSSQL_SA_PASSWORD` and the app's SQL Server connection string, then run the database migration steps in [Storage Provider](../Storage/Readme.md). The web service's `depends_on` entry should be removed if you are not running the SQL Server service.

```yml
  sql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sql
    expose:
      - 1433
    volumes:
      - sqlvolume:/var/opt/mssql
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=<your-sql-password>
    networks:
      - web_net
volumes: # creates a shared data volume named sqlvolume if you use sqlserver
  sqlvolume:
```
