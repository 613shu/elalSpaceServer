using ElAlProjectCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElAlProjectData.Configurations
{
    public class PassengerConfiguration : IEntityTypeConfiguration<Passenger>
    {
        public void Configure(EntityTypeBuilder<Passenger> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(256);
            builder.Property(p => p.Passward).IsRequired();

            builder.HasIndex(p => p.Email).IsUnique();
        }
    }
}
