using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

internal sealed class BrokenLinkConfiguration : IEntityTypeConfiguration<BrokenLink>
{
    public void Configure(EntityTypeBuilder<BrokenLink> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();
        builder.Property(b => b.BlogPostId).HasMaxLength(256).IsRequired();
        builder.Property(b => b.BlogPostTitle).HasMaxLength(256).IsRequired();
        builder.Property(b => b.Url).IsRequired();
        builder.Property(b => b.Reason).HasMaxLength(1024).IsRequired();
    }
}
