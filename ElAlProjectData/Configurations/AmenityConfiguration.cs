using ElAlProjectCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElAlProjectData.Configurations
{
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name).IsRequired().HasMaxLength(50);
        }
    }
}
