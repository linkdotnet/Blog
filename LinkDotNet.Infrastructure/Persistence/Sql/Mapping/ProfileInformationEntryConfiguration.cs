using LinkDotNet.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Infrastructure.Persistence.Sql.Mapping
{
    public class ProfileInformationEntryConfiguration : IEntityTypeConfiguration<ProfileInformationEntry>
    {
        public void Configure(EntityTypeBuilder<ProfileInformationEntry> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();
        }
    }
}