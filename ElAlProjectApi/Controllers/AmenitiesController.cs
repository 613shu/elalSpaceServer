using ElAlProjectCore.DTOs.RequstDTOs.AdminRequests;
using ElAlProjectCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElAlProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {


        private readonly IAmenityService _amenityService;

        public AmenitiesController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var amenity = await _amenityService.GetAmenityById(id, ct);
            return Ok(amenity);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var (items, totalCount) = await _amenityService.GetAllAmenities(page, pageSize, ct);
            return Ok(new { items, totalCount });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AdminRequest_AmenityDTO request, CancellationToken ct)
        {
            var amenity = await _amenityService.AddAmenity(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = amenity.Id }, amenity);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] AdminRequest_AmenityDTO request, CancellationToken ct)
        {
            var amenity = await _amenityService.UpdateAmenity(id, request, ct);
            return Ok(amenity);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        {
            await _amenityService.DeleteAmenity(id, ct);
            return NoContent();
        }
    }
}
