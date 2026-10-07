using ElAlProjectCore.Enums;
using ElAlProjectCore.Models;
using ElAlProjectData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ElAlProjectTests.Concurrency
{
    public class FlightConcurrencyTests : IAsyncLifetime
    {
        private const string ApiUserSecretsId = "1b4f962e-18f8-4eff-9dab-75f548d6d4bc";
        private const string TestDatabaseName = "ElAlTestDb";

        private readonly DbContextOptions<DataContext> _options;
        private int _flightId;

        public FlightConcurrencyTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(ApiUserSecretsId)
                .Build();

            var connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConnectionStrings:Default was not found in the user secrets of ElAlProjectApi.");

            var testConnectionString = new NpgsqlConnectionStringBuilder(connectionString) { Database = TestDatabaseName }.ConnectionString;

            _options = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(testConnectionString)
                .Options;
        }

        public async Task InitializeAsync()
        {
            await using var context = new DataContext(_options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();

            var flight = new Flight
            {
                FlightNumber = "LY001",
                DepartureAirport = "TLV",
                ArrivalAirport = "JFK",
                DepartureTime = DateTime.UtcNow.AddDays(1),
                ArrivalTime = DateTime.UtcNow.AddDays(1).AddHours(11),
                NumOfSeats = 100,
                AvailableSeats = 1,
                FlightStatus = FlightStatus.Scheduled,
                Price = 500
            };

            context.Flights.Add(flight);
            await context.SaveChangesAsync();
            _flightId = flight.Id;
        }

        public async Task DisposeAsync()
        {
            await using var context = new DataContext(_options);
            await context.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task TwoContexts_TakeLastSeat_FirstSaveSucceedsSecondThrows()
        {
            await using var firstContext = new DataContext(_options);
            await using var secondContext = new DataContext(_options);

            var firstFlight = await firstContext.Flights.SingleAsync(f => f.Id == _flightId);
            var secondFlight = await secondContext.Flights.SingleAsync(f => f.Id == _flightId);

            firstFlight.AvailableSeats--;
            secondFlight.AvailableSeats--;

            await firstContext.SaveChangesAsync();

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secondContext.SaveChangesAsync());

            await using var checkContext = new DataContext(_options);
            var savedFlight = await checkContext.Flights.AsNoTracking().SingleAsync(f => f.Id == _flightId);
            Assert.Equal(0, savedFlight.AvailableSeats);
        }
    }
}
