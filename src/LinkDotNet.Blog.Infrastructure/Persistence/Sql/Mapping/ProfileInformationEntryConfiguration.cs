using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

public sealed class ProfileInformationEntryConfiguration : IEntityTypeConfiguration<ProfileInformationEntry>
{
    public void Configure(EntityTypeBuilder<ProfileInformationEntry> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();
        builder.Property(c => c.Content).HasMaxLength(512).IsRequired();
        builder.Property(c => c.SortOrder).IsRequired();
    }
}
