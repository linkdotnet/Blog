using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

internal sealed class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
    }
}
