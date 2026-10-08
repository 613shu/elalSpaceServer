using ElAlProjectCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElAlProjectData.Configurations
{
    public class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.FlightNumber).IsRequired().HasMaxLength(20);
            builder.Property(f => f.DepartureAirport).IsRequired().HasMaxLength(100);
            builder.Property(f => f.ArrivalAirport).IsRequired().HasMaxLength(100);

            builder.Property(f => f.Version).IsRowVersion();

            builder.HasMany(f => f.Amenities)
                .WithMany(a => a.Flights)
                .UsingEntity("AmenityFlight");
        }
    }
}
