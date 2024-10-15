using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

internal sealed class ShortCodeConfiguration : IEntityTypeConfiguration<ShortCode>
{
    public void Configure(EntityTypeBuilder<ShortCode> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(512);
        builder.Property(s => s.MarkdownContent)
            .IsRequired();
    }
}
