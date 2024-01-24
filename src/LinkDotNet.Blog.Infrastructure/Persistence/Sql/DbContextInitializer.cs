using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;
public sealed partial class DbContextInitializer
{
    private readonly ILogger<DbContextInitializer> logger;

    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;

    private readonly BlogDbContext dbContext;

    public DbContextInitializer(ILogger<DbContextInitializer> logger, IDbContextFactory<BlogDbContext> dbContextFactory)
    {
        this.logger = logger;

        this.dbContextFactory = dbContextFactory;

        dbContext = this.dbContextFactory.CreateDbContext();
    }

    public void Initialize()
    {
        try
        {
            if (StorageProviderIsSQL())
            {
                dbContext.Database.Migrate();
                LogInitializingInfo();
            }
        }
        catch
        {
            LogInitializingError();
            throw;
        }
    }

    [LoggerMessage(LogLevel.Error, "An error occurred while initializing the database.")]
    private partial void LogInitializingError();

    [LoggerMessage(LogLevel.Information, "Database migrated.")]
    private partial void LogInitializingInfo();

    private bool StorageProviderIsSQL()
    {
        if (dbContext.Database.IsMySql() || dbContext.Database.IsSqlServer() || dbContext.Database.IsSqlite())
        {
            return true;
        }

        return false;
    }
}
