using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql;

public sealed class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<BlogPost> BlogPosts { get; set; }

    public DbSet<Tag> Tags { get; set; }

    public DbSet<ProfileInformationEntry> ProfileInformationEntries { get; set; }

    public DbSet<Skill> Skills { get; set; }

    public DbSet<UserRecord> UserRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BlogPostConfiguration());
        modelBuilder.ApplyConfiguration(new TagsConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileInformationEntryConfiguration());
        modelBuilder.ApplyConfiguration(new SkillConfiguration());
        modelBuilder.ApplyConfiguration(new UserRecordConfiguration());
    }
}
