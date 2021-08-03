using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Infrastructure.Persistence.Sql.Mapping
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.HasMany(t => t.Tags)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(x => x.Tags).AutoInclude();
        }
    }
}