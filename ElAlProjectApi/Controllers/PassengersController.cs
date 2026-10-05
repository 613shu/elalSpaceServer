using ElAlProjectCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAlProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PassengersController : ControllerBase
    {
        private readonly IPassengerService _passengerService;

        public PassengersController(IPassengerService passengerService)
        {
            _passengerService = passengerService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetPassengerById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var passenger = await _passengerService.GetPassengerById(id, cancellationToken);

            return Ok(passenger);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPassengers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, totalCount) = await _passengerService.GetAllPassengers(page, pageSize, cancellationToken);

            return Ok(new { items, totalCount });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePassenger([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _passengerService.DeletePassenger(id, cancellationToken);

            return NoContent();
        }
    }
}
