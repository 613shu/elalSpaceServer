using ElAlProjectCore.DTOs.RequstDTOs.PassengerRequest;
using ElAlProjectCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElAlProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin,Passenger")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsAdmin => User.IsInRole("Admin");

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var order = await _orderService.GetOrderById(id, CurrentUserId, IsAdmin, ct);
            return Ok(order);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Passenger")]
        public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var (items, totalCount) = await _orderService.GetPassengerOrders(CurrentUserId, page, pageSize, ct);
            return Ok(new { items, totalCount });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var (items, totalCount) = await _orderService.GetAllOrders(page, pageSize, ct);
            return Ok(new { items, totalCount });
        }

        [HttpPost]
        [Authorize(Roles = "Passenger")]
        public async Task<IActionResult> Create([FromBody] PassengerRequest_OrderDTO request, CancellationToken ct)
        {
            var order = await _orderService.AddOrder(CurrentUserId, request, ct);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel([FromRoute] int id, CancellationToken ct)
        {
            await _orderService.DeleteOrder(id, CurrentUserId, IsAdmin, ct);
            return NoContent();
        }

    }
}
