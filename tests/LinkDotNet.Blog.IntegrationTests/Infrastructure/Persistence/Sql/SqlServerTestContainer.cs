using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

internal static class SqlServerTestContainer
{
    private static readonly TestContainer Container = TestContainer.Create("SQL Server", () => new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build(), c => c.GetConnectionString());

    public static async Task<string> CreateDatabaseConnectionStringAsync()
    {
        if (OperatingSystem.IsLinux() && RuntimeInformation.OSArchitecture == Architecture.Arm64)
        {
            Assert.Skip("SQL Server images are only published for x64.");
        }

        var builder = new SqlConnectionStringBuilder(await Container.GetConnectionStringAsync())
        {
            InitialCatalog = "test-" + Guid.NewGuid().ToString("N"),
        };
        return builder.ConnectionString;
    }
}
