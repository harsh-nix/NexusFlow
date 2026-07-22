using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusFlow.Application.DTOs.Comments;
using NexusFlow.Application.Services.Interfaces;
using System.Security.Claims;

namespace NexusFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
        {
            var result = await _commentService.CreateAsync(dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetByTask(int taskId)
        {
            var result = await _commentService.GetByTaskAsync(taskId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _commentService.DeleteAsync(id, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("task/{taskId}/clarification")]
        public async Task<IActionResult> RequestClarification(
            int taskId, [FromBody] RequestClarificationDto dto)
        {
            var result = await _commentService.RequestClarificationAsync(taskId, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("task/{taskId}/clarification/respond")]
        public async Task<IActionResult> RespondToClarification(
            int taskId, [FromBody] RespondClarificationDto dto)
        {
            var result = await _commentService.RespondToClarificationAsync(taskId, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }
    }
}