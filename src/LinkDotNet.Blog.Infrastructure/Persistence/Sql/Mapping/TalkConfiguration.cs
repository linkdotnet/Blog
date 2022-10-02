using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

public class TalkConfiguration : IEntityTypeConfiguration<Talk>
{
    public void Configure(EntityTypeBuilder<Talk> builder)
    {
        builder.Property(t => t.PresentationTitle).HasMaxLength(256).IsRequired();
        builder.Property(t => t.Place).HasMaxLength(256).IsRequired();
        builder.Property(t => t.PublishedDate).IsRequired();
    }
}