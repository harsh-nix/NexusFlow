using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Reports;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IReportService
    {
        Task<ApiResponse<List<ProjectReportRowDto>>> GetProjectReportDataAsync();
        Task<ApiResponse<List<TaskReportRowDto>>> GetTaskReportDataAsync(int projectId);

        Task<byte[]> ExportProjectReportExcelAsync(int userId);
        Task<byte[]> ExportProjectReportPdfAsync(int userId);
        Task<byte[]> ExportTaskReportExcelAsync(int projectId, int userId);
        Task<byte[]> ExportTaskReportPdfAsync(int projectId, int userId);
    }
}