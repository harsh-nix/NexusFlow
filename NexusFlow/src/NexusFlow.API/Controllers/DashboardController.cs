using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusFlow.Application.Services.Interfaces;
using System.Security.Claims;

namespace NexusFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            var result = await _dashboardService.GetStatsAsync(GetUserId());
            return StatusCode(result.StatusCode, result);
        }
    }
}