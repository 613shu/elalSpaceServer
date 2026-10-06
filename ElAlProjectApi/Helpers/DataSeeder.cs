using ElAlProjectCore.Enums;
using ElAlProjectCore.Models;
using ElAlProjectData;
using ElAlProjectService.Security;
using Microsoft.EntityFrameworkCore;

namespace ElAlProjectApi.Helpers
{
    public class DataSeeder
    {






        public static async Task SeedAsync(DataContext context)
        {

            if (!await context.Admins.AnyAsync())
            {
                context.Admins.Add(new Admin
                {
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    Password = PasswordHasher.Hash("111222")
                });
            }

            if (!await context.Flights.AnyAsync())
            {
                var amenities = await context.Amenities.ToListAsync();
                if (amenities.Count == 0)
                {
                    amenities = new[] { "חלון פרטי", "כבידה מדומה", "ארוחות שף", "ציוד אישי כלול", "הכשרה לפני ההמראה", "סלון משותף" }
                        .Select(name => new Amenity { Name = name })
                        .ToList();
                    context.Amenities.AddRange(amenities);
                }

                var destinations = new[]
                {
                    (Name: "הירח", Days: 3, Price: 184000.0, Seats: 24),
                    (Name: "תחנת המסלול", Days: 1, Price: 95000.0, Seats: 30),
                    (Name: "מאדים", Days: 210, Price: 2400000.0, Seats: 12),
                    (Name: "אירופה", Days: 600, Price: 5200000.0, Seats: 8),
                    (Name: "שבתאי", Days: 900, Price: 7800000.0, Seats: 6)
                };

                var today = DateTime.UtcNow.Date;
                var flights = new List<Flight>();

                for (int i = 0; i < 25; i++)
                {
                    var destination = destinations[i % destinations.Length];
                    var departure = today.AddDays(7 + i * 6).AddHours(6);

                    flights.Add(new Flight
                    {
                        FlightNumber = $"ES {i + 1:000}",
                        DepartureAirport = "תל אביב",
                        ArrivalAirport = destination.Name,
                        DepartureTime = departure,
                        ArrivalTime = departure.AddDays(destination.Days),
                        NumOfSeats = destination.Seats,
                        AvailableSeats = destination.Seats,
                        FlightStatus = FlightStatus.Scheduled,
                        Price = destination.Price,
                        Amenities = amenities.Where((a, index) => (index + i) % 2 == 0).ToList()
                    });
                }

                flights.Add(EdgeCase("ES 901", today.AddDays(10), 24, 1, FlightStatus.Scheduled));
                flights.Add(EdgeCase("ES 902", today.AddDays(12), 24, 0, FlightStatus.Scheduled));
                flights.Add(EdgeCase("ES 903", today.AddDays(14), 24, 24, FlightStatus.Cancelled));
                flights.Add(EdgeCase("ES 904", today.AddDays(-1), 24, 5, FlightStatus.Scheduled));
                flights.Add(EdgeCase("ES 905", today.AddDays(-30), 24, 0, FlightStatus.Completed));

                context.Flights.AddRange(flights);
            }

            await context.SaveChangesAsync();
        }

        private static Flight EdgeCase(string number, DateTime departure, int seats, int available, FlightStatus status)
        {
            return new Flight
            {
                FlightNumber = number,
                DepartureAirport = "תל אביב",
                ArrivalAirport = "הירח",
                DepartureTime = departure,
                ArrivalTime = departure.AddDays(3),
                NumOfSeats = seats,
                AvailableSeats = available,
                FlightStatus = status,
                Price = 184000
            };
        }

    }
}


