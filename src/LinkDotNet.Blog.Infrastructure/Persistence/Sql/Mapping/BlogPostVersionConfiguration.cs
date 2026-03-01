using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

internal sealed class BlogPostVersionConfiguration : IEntityTypeConfiguration<BlogPostVersion>
{
    public void Configure(EntityTypeBuilder<BlogPostVersion> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.BlogPostId).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Version).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(256).IsRequired();
        builder.Property(x => x.PreviewImageUrl).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.PreviewImageUrlFallback).HasMaxLength(1024);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.ShortDescription).IsRequired();
        builder.Property(x => x.IsPublished).IsRequired();
        builder.Property(x => x.ReadingTimeInMinutes).IsRequired();
        builder.Property(x => x.Tags).HasMaxLength(2048);
        builder.Property(x => x.AuthorName).HasMaxLength(256).IsRequired(false);

        builder.HasIndex(x => new { x.BlogPostId, x.Version })
            .IsUnique();
    }
}
