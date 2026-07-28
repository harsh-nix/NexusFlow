using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Files;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Enums;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IConfiguration _configuration;

        private static readonly string[] AllowedExtensions =
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            ".png", ".jpg", ".jpeg", ".gif", ".txt", ".zip", ".csv"
        };

        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public FileService(
            IUnitOfWork unitOfWork,
            IAuditLogService auditLogService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<List<FileAttachmentDto>>> GetByTaskAsync(int taskId)
        {
            var files = await _unitOfWork.Repository<FileAttachment>()
                .FindAsync(f => f.TaskId == taskId && !f.IsDeleted);

            var result = new List<FileAttachmentDto>();
            foreach (var file in files.OrderByDescending(f => f.CreatedAt))
            {
                result.Add(await ToDtoAsync(file));
            }

            return ApiResponse<List<FileAttachmentDto>>.Ok(result);
        }

        public async Task<ApiResponse<FileAttachmentDto>> UploadAsync(
            int taskId, IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return ApiResponse<FileAttachmentDto>.Fail("No file was provided.", 400);

            if (file.Length > MaxFileSizeBytes)
                return ApiResponse<FileAttachmentDto>.Fail("File exceeds the 10 MB limit.", 400);

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return ApiResponse<FileAttachmentDto>.Fail(
                    $"File type '{extension}' is not allowed.", 400);

            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == taskId && !t.IsDeleted);
            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<FileAttachmentDto>.Fail("Task not found.", 404);

            var uploadRoot = GetUploadRoot();
            Directory.CreateDirectory(uploadRoot);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadRoot, storedFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new FileAttachment
            {
                StoredFileName = storedFileName,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length,
                TaskId = taskId,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<FileAttachment>().AddAsync(attachment);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", taskId, "FileUploaded", null, attachment.OriginalFileName, userId);

            return ApiResponse<FileAttachmentDto>.Created(
                await ToDtoAsync(attachment), "File uploaded.");
        }

        public async Task<ApiResponse<FileDownloadResult>> DownloadAsync(int id, int userId)
        {
            var files = await _unitOfWork.Repository<FileAttachment>()
                .FindAsync(f => f.Id == id && !f.IsDeleted);
            var file = files.FirstOrDefault();

            if (file == null)
                return ApiResponse<FileDownloadResult>.Fail("File not found.", 404);

            var fullPath = Path.Combine(GetUploadRoot(), file.StoredFileName);

            if (!File.Exists(fullPath))
                return ApiResponse<FileDownloadResult>.Fail(
                    "The file is missing from storage.", 404);

            var bytes = await File.ReadAllBytesAsync(fullPath);

            return ApiResponse<FileDownloadResult>.Ok(new FileDownloadResult
            {
                Content = bytes,
                ContentType = file.ContentType,
                FileName = file.OriginalFileName
            });
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
        {
            var files = await _unitOfWork.Repository<FileAttachment>()
                .FindAsync(f => f.Id == id && !f.IsDeleted);
            var file = files.FirstOrDefault();

            if (file == null)
                return ApiResponse<bool>.Fail("File not found.", 404);

            if (file.CreatedBy != userId)
            {
                var users = await _unitOfWork.Repository<User>()
                    .FindAsync(u => u.Id == userId);
                var caller = users.FirstOrDefault();

                if (caller?.Role != UserRole.Admin && caller?.Role != UserRole.ProjectManager)
                    return ApiResponse<bool>.Fail(
                        "You are not authorized to delete this file.", 403);
            }

            file.IsDeleted = true;
            file.UpdatedBy = userId;
            _unitOfWork.Repository<FileAttachment>().Update(file);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", file.TaskId, "FileRemoved", file.OriginalFileName, null, userId);

            return ApiResponse<bool>.Ok(true, "File removed.");
        }

        private string GetUploadRoot()
        {
            var configuredPath = _configuration["FileStorage:UploadPath"] ?? "Uploads";
            return Path.IsPathRooted(configuredPath)
                ? configuredPath
                : Path.Combine(Directory.GetCurrentDirectory(), configuredPath);
        }

        private async Task<FileAttachmentDto> ToDtoAsync(FileAttachment file)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == file.CreatedBy);

            return new FileAttachmentDto
            {
                Id = file.Id,
                OriginalFileName = file.OriginalFileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.FileSizeBytes,
                TaskId = file.TaskId,
                UploadedBy = file.CreatedBy,
                UploadedByName = users.FirstOrDefault()?.FullName ?? "Unknown",
                CreatedAt = file.CreatedAt
            };
        }
    }
}