using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusFlow.Application.Services.Interfaces;
using System.Security.Claims;

namespace NexusFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var result = await _notificationService.GetByUserAsync(GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var result = await _notificationService.MarkAllAsReadAsync(GetUserId());
            return StatusCode(result.StatusCode, result);
        }
    }
}