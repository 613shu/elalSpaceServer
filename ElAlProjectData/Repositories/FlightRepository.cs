using ElAlProjectCore.Enums;
using ElAlProjectCore.Models;
using ElAlProjectCore.Repositories;
using ElAlProjectData.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElAlProjectData.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly DataContext _dataContext;


        public FlightRepository(DataContext dataContext)
        {
            _dataContext= dataContext;
        }



        public async Task<Flight> AddFlight(Flight flight, CancellationToken cancellationToken)
        {
            await _dataContext.Flights.AddAsync(flight, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return flight;
        }





        public async Task<(IEnumerable<Flight> Items, int TotalCount)> GetAvailableFlightsPaged(DateTime departsAfter, int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<Flight> query = _dataContext.Flights.AsNoTracking()
                .Include(f => f.Amenities)
                .Where(f => f.FlightStatus == FlightStatus.Scheduled && f.AvailableSeats > 0 && f.DepartureTime > departsAfter)
                .OrderBy(f => f.DepartureTime)
                .ThenBy(f => f.Id);
            return await query.ToPagedAsync(page, pageSize, cancellationToken);

        }



        public async Task<Flight?> GetFlightById(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Flights.AsNoTracking()
                .Include(f => f.Amenities)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }



        public async Task<Flight?> GetFlightForUpdate(int id, CancellationToken cancellationToken)
        {
            return await _dataContext.Flights
                .Include(f => f.Amenities)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public void ClearTracking()
        {
            _dataContext.ChangeTracker.Clear();
        }



        //pagination
        // All flights, in every status (for the admin)
        public async Task<(IEnumerable<Flight> Items, int TotalCount)> GetAllFlightsPaged(int page, int pageSize, CancellationToken cancellationToken)
        {
            IQueryable<Flight> query = _dataContext.Flights.AsNoTracking()
                .Include(f => f.Amenities)
                .OrderBy(f => f.DepartureTime)
                .ThenBy(f => f.Id);

            return await query.ToPagedAsync(page, pageSize, cancellationToken);
        }



        // Updates the flight details only. Seats and status are not changed here.
        // Returns null when the flight does not exist
        public async Task<Flight?> UpdateFlight(int id, Flight flight, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Flights
                .Include(f => f.Amenities)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

            if (existing == null)
                return null;

            existing.FlightNumber = flight.FlightNumber;
            existing.DepartureAirport = flight.DepartureAirport;
            existing.ArrivalAirport = flight.ArrivalAirport;
            existing.DepartureTime = flight.DepartureTime;
            existing.ArrivalTime = flight.ArrivalTime;
            existing.Price = flight.Price;
            existing.Amenities = flight.Amenities;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return existing;
        }



        // Soft delete: the flight stays in the database with status Cancelled.
        // Returns false when the flight does not exist or is already cancelled
        public async Task<bool> DeleteFlight(int id, CancellationToken cancellationToken)
        {
            var existing = await _dataContext.Flights
                .FirstOrDefaultAsync(f => f.Id == id && f.FlightStatus != FlightStatus.Cancelled, cancellationToken);

            if (existing == null)
                return false;

            existing.FlightStatus = FlightStatus.Cancelled;

            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
