using ElAlProjectCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElAlProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAdminById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var admin = await _adminService.GetAdminById(id, cancellationToken);

            return Ok(admin);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAdmins([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var (items, totalCount) = await _adminService.GetAllAdmins(page, pageSize, cancellationToken);

            return Ok(new { items, totalCount });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdmin([FromRoute] int id, CancellationToken cancellationToken)
        {
            await _adminService.DeleteAdmin(id, cancellationToken);

            return NoContent();
        }
    }
}
