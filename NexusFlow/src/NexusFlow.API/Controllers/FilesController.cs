using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusFlow.Application.Services.Interfaces;
using System.Security.Claims;

namespace NexusFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetByTask(int taskId)
        {
            var result = await _fileService.GetByTaskAsync(taskId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("task/{taskId}")]
        public async Task<IActionResult> Upload(int taskId, IFormFile file)
        {
            var result = await _fileService.UploadAsync(taskId, file, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            var result = await _fileService.DownloadAsync(id, GetUserId());

            if (!result.Success || result.Data == null)
                return StatusCode(result.StatusCode, result);

            return File(result.Data.Content, result.Data.ContentType, result.Data.FileName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _fileService.DeleteAsync(id, GetUserId());
            return StatusCode(result.StatusCode, result);
        }
    }
}