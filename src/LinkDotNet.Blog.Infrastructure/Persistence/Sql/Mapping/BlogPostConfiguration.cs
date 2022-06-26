using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.HasMany(t => t.Tags)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(x => x.Tags).AutoInclude();
        builder.Property(x => x.Title).HasMaxLength(256).IsRequired();
        builder.Property(x => x.PreviewImageUrl).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.PreviewImageUrlFallback).HasMaxLength(1024);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.ShortDescription).IsRequired();
        builder.Property(x => x.Likes).IsRequired();
        builder.Property(x => x.IsPublished).IsRequired();
    }
}