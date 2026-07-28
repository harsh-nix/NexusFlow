using Microsoft.AspNetCore.Http;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Files;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IFileService
    {
        Task<ApiResponse<List<FileAttachmentDto>>> GetByTaskAsync(int taskId);
        Task<ApiResponse<FileAttachmentDto>> UploadAsync(int taskId, IFormFile file, int userId);
        Task<ApiResponse<FileDownloadResult>> DownloadAsync(int id, int userId);
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);
    }
}