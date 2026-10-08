using ElAlProjectCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElAlProjectData.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Email).IsRequired().HasMaxLength(256);
            builder.Property(a => a.Password).IsRequired();

            builder.HasIndex(a => a.Email).IsUnique();
        }
    }
}
