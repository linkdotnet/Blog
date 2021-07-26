using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinkDotNet.Infrastructure.Persistence.Sql
{
    public sealed class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<ProfileInformationEntry> ProfileInformationEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<BlogPost>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<BlogPost>()
                .HasMany(t => t.Tags)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tag>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ProfileInformationEntry>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<ProfileInformationEntry>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
        }
    }
}