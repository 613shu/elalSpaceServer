
        using ElAlProjectCore.DTOs;
using ElAlProjectCore.DTOs.RequstDTOs.AdminRequests;
using ElAlProjectCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAlProjectApi.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var flight = await _flightService.GetFlightById(id, ct);
            return Ok(flight);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var (items, totalCount) = await _flightService.GetAllFlights(page, pageSize, ct);
            return Ok(new { items, totalCount });
        }

        [AllowAnonymous]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var (items, totalCount) = await _flightService.GetAvailableFlights(page, pageSize, ct);
            return Ok(new { items, totalCount });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AdminRequest_FlightDTO request, CancellationToken ct)
        {
            var flight = await _flightService.AddFlight(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = flight.Id }, flight);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AdminRequest_FlightDTO request, CancellationToken ct)
        {
            var flight = await _flightService.UpdateFlight(id, request, ct);
            return Ok(flight);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            await _flightService.DeleteFlight(id, ct);
            return NoContent();
        }
    }


