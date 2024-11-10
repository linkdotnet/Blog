using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

internal sealed class SimilarBlogPostConfiguration : IEntityTypeConfiguration<SimilarBlogPost>
{
    public void Configure(EntityTypeBuilder<SimilarBlogPost> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();

        builder
            .Property(b => b.SimilarBlogPostIds)
            .HasMaxLength(450 * 3)
            .IsRequired();
    }
}
