using System;
using LinkDotNet.Blog.Domain;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogDbContext).Assembly);
    }
}
