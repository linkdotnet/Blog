using System.Runtime.CompilerServices;
using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

public class BlogPostRecordConfiguration : IEntityTypeConfiguration<BlogPostRecord>
{
    public void Configure(EntityTypeBuilder<BlogPostRecord> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();
        builder.Property(s => s.BlogPostId).HasMaxLength(256).IsRequired();
    }
}
