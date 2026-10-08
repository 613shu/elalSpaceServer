using ElAlProjectCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElAlProjectData.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.Flight)
                .WithMany()
                .HasForeignKey(o => o.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Passenger)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PassengerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
