using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusFlow.Application.DTOs.Tasks;
using NexusFlow.Application.Services.Interfaces;
using System.Security.Claims;

namespace NexusFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubTasksController : ControllerBase
    {
        private readonly ISubTaskService _subTaskService;

        public SubTasksController(ISubTaskService subTaskService)
        {
            _subTaskService = subTaskService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetByTask(int taskId)
        {
            var result = await _subTaskService.GetByTaskAsync(taskId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("task/{taskId}")]
        public async Task<IActionResult> Create(int taskId, [FromBody] CreateSubTaskDto dto)
        {
            var result = await _subTaskService.CreateAsync(taskId, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSubTaskDto dto)
        {
            var result = await _subTaskService.UpdateAsync(id, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _subTaskService.DeleteAsync(id, GetUserId());
            return StatusCode(result.StatusCode, result);
        }
    }
}