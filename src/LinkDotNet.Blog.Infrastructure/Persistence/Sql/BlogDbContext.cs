using System;
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

    public DbSet<ProfileInformationEntry> ProfileInformationEntries { get; set; }

    public DbSet<Skill> Skills { get; set; }

    public DbSet<Talk> Talks { get; set; }

    public DbSet<UserRecord> UserRecords { get; set; }

    public DbSet<BlogPostRecord> BlogPostRecords { get; set; }

    public DbSet<SimilarBlogPost> SimilarBlogPosts { get; set; }

    public DbSet<ShortCode> ShortCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new BlogPostConfiguration(Database));
        modelBuilder.ApplyConfiguration(new BlogPostRecordConfiguration());
        modelBuilder.ApplyConfiguration(new ProfileInformationEntryConfiguration());
        modelBuilder.ApplyConfiguration(new ShortCodeConfiguration());
        modelBuilder.ApplyConfiguration(new SimilarBlogPostConfiguration(Database));
        modelBuilder.ApplyConfiguration(new SkillConfiguration());
        modelBuilder.ApplyConfiguration(new TalkConfiguration());
        modelBuilder.ApplyConfiguration(new UserRecordConfiguration());
    }
}
