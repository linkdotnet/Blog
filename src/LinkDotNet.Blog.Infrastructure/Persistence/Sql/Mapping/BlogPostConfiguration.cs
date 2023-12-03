using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

internal sealed class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<BlogPostIdGenerator>();
        builder.Property(x => x.Title).HasMaxLength(256).IsRequired();
        builder.Property(x => x.PreviewImageUrl).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.PreviewImageUrlFallback).HasMaxLength(1024);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.ShortDescription).IsRequired();
        builder.Property(x => x.Likes).IsRequired();
        builder.Property(x => x.IsPublished).IsRequired();
        builder.Property(x => x.Tags).HasMaxLength(2096);

        builder.HasIndex(x => new { x.IsPublished, x.UpdatedDate })
            .HasDatabaseName("IX_BlogPosts_IsPublished_UpdatedDate")
            .IsDescending(false, true);
    }
}


internal sealed class BlogPostIdGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry)
    {
        return Guid.NewGuid().ToString("N").Substring(0, 15);
    }
}
