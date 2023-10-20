using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

internal sealed class TalkConfiguration : IEntityTypeConfiguration<Talk>
{
    public void Configure(EntityTypeBuilder<Talk> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();
        builder.Property(t => t.PresentationTitle).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Place).HasMaxLength(256).IsRequired();
        builder.Property(t => t.PublishedDate).IsRequired();
    }
}
