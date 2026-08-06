using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusFlow.Application.Services.Interfaces;
using System.Security.Claims;

namespace NexusFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,ProjectManager")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjectReportData()
        {
            var result = await _reportService.GetProjectReportDataAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("projects/excel")]
        public async Task<IActionResult> ExportProjectReportExcel()
        {
            var bytes = await _reportService.ExportProjectReportExcelAsync(GetUserId());
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ProjectReport.xlsx");
        }

        [HttpGet("projects/pdf")]
        public async Task<IActionResult> ExportProjectReportPdf()
        {
            var bytes = await _reportService.ExportProjectReportPdfAsync(GetUserId());
            return File(bytes, "application/pdf", "ProjectReport.pdf");
        }

        [HttpGet("tasks/{projectId}")]
        public async Task<IActionResult> GetTaskReportData(int projectId)
        {
            var result = await _reportService.GetTaskReportDataAsync(projectId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("tasks/{projectId}/excel")]
        public async Task<IActionResult> ExportTaskReportExcel(int projectId)
        {
            var bytes = await _reportService.ExportTaskReportExcelAsync(projectId, GetUserId());
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "TaskReport.xlsx");
        }

        [HttpGet("tasks/{projectId}/pdf")]
        public async Task<IActionResult> ExportTaskReportPdf(int projectId)
        {
            var bytes = await _reportService.ExportTaskReportPdfAsync(projectId, GetUserId());
            return File(bytes, "application/pdf", "TaskReport.pdf");
        }
    }
}