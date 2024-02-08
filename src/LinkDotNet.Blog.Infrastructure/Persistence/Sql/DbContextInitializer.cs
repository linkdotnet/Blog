using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

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
            var database = dbContext.Database;

            if (database.GetPendingMigrations().Any())
            {
                database.Migrate();
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
}
