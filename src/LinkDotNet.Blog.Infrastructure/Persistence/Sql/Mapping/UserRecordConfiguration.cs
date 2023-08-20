using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

public sealed class UserRecordConfiguration : IEntityTypeConfiguration<UserRecord>
{
    public void Configure(EntityTypeBuilder<UserRecord> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();
        builder.Property(s => s.UrlClicked).HasMaxLength(256).IsRequired();
    }
}
