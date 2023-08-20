using LinkDotNet.Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkDotNet.Blog.Infrastructure.Persistence.Sql.Mapping;

public sealed class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .IsUnicode(false)
            .ValueGeneratedOnAdd();
        builder.Property(s => s.ProficiencyLevel)
            .HasConversion(to => to.Key, from => ProficiencyLevel.Create(from))
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(s => s.Name).HasMaxLength(128).IsRequired();
        builder.Property(s => s.IconUrl).HasMaxLength(1024);
        builder.Property(s => s.Capability).HasMaxLength(128).IsRequired();
    }
}
