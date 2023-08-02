## Host Web in Docker containers

### Server configuration

To deploy with docker, you need to modify the variables in the docker-compose.yml file.

```yml
volumes:
      - /root/.aspnet/DataProtection-Keys:/root/.aspnet/DataProtection-Keys
      - ./Blog.db:/app/Blog.db # Sqlite datebase consistent with appsettings.json
      - /root/aspnetapp.pfx:/app/aspnetapp.pfx # ssl certificate
    environment:
      - ASPNETCORE_URLS=http://+:80;https://+:443
      - ASPNETCORE_HTTPS_PORT=80
      - ASPNETCORE_Kestrel__Certificates__Default__Password= # Your certificate password
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx
      - ASPNETCORE_ENVIRONMENT=Production
```

After modifying the settings, you can use the docker command `docker compose up -d`
Deploy the web.
If you don't use HTTPS, you can remove the related options.
SQL Server

If you use SQL Server, you can add an instance in `docker-compose.yml`.

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
      - MSSQL_SA_PASSWORD=  # Your sql password
    networks:
      - web_net
volumes: # creates a shared data volume named sqlvolume if you use sqlserver
  sqlvolume:
```
