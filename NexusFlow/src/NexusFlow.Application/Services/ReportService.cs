using ClosedXML.Excel;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Reports;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace NexusFlow.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public ReportService(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        public async Task<ApiResponse<List<ProjectReportRowDto>>> GetProjectReportDataAsync()
        {
            var rows = await BuildProjectReportRowsAsync();
            return ApiResponse<List<ProjectReportRowDto>>.Ok(rows);
        }

        public async Task<ApiResponse<List<TaskReportRowDto>>> GetTaskReportDataAsync(int projectId)
        {
            var rows = await BuildTaskReportRowsAsync(projectId);
            return ApiResponse<List<TaskReportRowDto>>.Ok(rows);
        }

        public async Task<byte[]> ExportProjectReportExcelAsync(int userId)
        {
            var rows = await BuildProjectReportRowsAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Projects");

            sheet.Cell(1, 1).Value = "Project Name";
            sheet.Cell(1, 2).Value = "Status";
            sheet.Cell(1, 3).Value = "Members";
            sheet.Cell(1, 4).Value = "Tasks";
            sheet.Cell(1, 5).Value = "Completed";
            sheet.Cell(1, 6).Value = "Progress %";
            sheet.Cell(1, 7).Value = "Created";
            sheet.Row(1).Style.Font.Bold = true;

            var row = 2;
            foreach (var item in rows)
            {
                sheet.Cell(row, 1).Value = item.Name;
                sheet.Cell(row, 2).Value = item.Status;
                sheet.Cell(row, 3).Value = item.MemberCount;
                sheet.Cell(row, 4).Value = item.TaskCount;
                sheet.Cell(row, 5).Value = item.CompletedTaskCount;
                sheet.Cell(row, 6).Value = item.ProgressPercent;
                sheet.Cell(row, 7).Value = item.CreatedAt.ToString("yyyy-MM-dd");
                row++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            await _auditLogService.LogAsync(
                "Report", 0, "ProjectReportExported", null, "Excel", userId);

            return stream.ToArray();
        }

        public async Task<byte[]> ExportProjectReportPdfAsync(int userId)
        {
            var rows = await BuildProjectReportRowsAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text("NexusFlow — Project Report").FontSize(18).Bold();

                    page.Content().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Project").Bold();
                            header.Cell().Text("Status").Bold();
                            header.Cell().Text("Tasks").Bold();
                            header.Cell().Text("Done").Bold();
                            header.Cell().Text("Progress").Bold();
                        });

                        foreach (var item in rows)
                        {
                            table.Cell().Text(item.Name);
                            table.Cell().Text(item.Status);
                            table.Cell().Text(item.TaskCount.ToString());
                            table.Cell().Text(item.CompletedTaskCount.ToString());
                            table.Cell().Text($"{item.ProgressPercent}%");
                        }
                    });

                    page.Footer().AlignRight().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            await _auditLogService.LogAsync(
                "Report", 0, "ProjectReportExported", null, "PDF", userId);

            return document.GeneratePdf();
        }

        public async Task<byte[]> ExportTaskReportExcelAsync(int projectId, int userId)
        {
            var rows = await BuildTaskReportRowsAsync(projectId);

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Tasks");

            sheet.Cell(1, 1).Value = "Title";
            sheet.Cell(1, 2).Value = "Status";
            sheet.Cell(1, 3).Value = "Priority";
            sheet.Cell(1, 4).Value = "Assignees";
            sheet.Cell(1, 5).Value = "Due Date";
            sheet.Cell(1, 6).Value = "Created By";
            sheet.Row(1).Style.Font.Bold = true;

            var row = 2;
            foreach (var item in rows)
            {
                sheet.Cell(row, 1).Value = item.Title;
                sheet.Cell(row, 2).Value = item.Status;
                sheet.Cell(row, 3).Value = item.Priority;
                sheet.Cell(row, 4).Value = item.Assignees;
                sheet.Cell(row, 5).Value = item.DueDate ?? "";
                sheet.Cell(row, 6).Value = item.CreatedByName;
                row++;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            await _auditLogService.LogAsync(
                "Report", projectId, "TaskReportExported", null, "Excel", userId);

            return stream.ToArray();
        }

        public async Task<byte[]> ExportTaskReportPdfAsync(int projectId, int userId)
        {
            var rows = await BuildTaskReportRowsAsync(projectId);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text("NexusFlow — Task Report").FontSize(18).Bold();

                    page.Content().PaddingTop(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Title").Bold();
                            header.Cell().Text("Status").Bold();
                            header.Cell().Text("Priority").Bold();
                            header.Cell().Text("Assignees").Bold();
                            header.Cell().Text("Due").Bold();
                        });

                        foreach (var item in rows)
                        {
                            table.Cell().Text(item.Title);
                            table.Cell().Text(item.Status);
                            table.Cell().Text(item.Priority);
                            table.Cell().Text(item.Assignees);
                            table.Cell().Text(item.DueDate ?? "—");
                        }
                    });

                    page.Footer().AlignRight().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            await _auditLogService.LogAsync(
                "Report", projectId, "TaskReportExported", null, "PDF", userId);

            return document.GeneratePdf();
        }

        // ---------- Helpers ----------

        private async Task<List<ProjectReportRowDto>> BuildProjectReportRowsAsync()
        {
            var projects = await _unitOfWork.Repository<Project>()
                .FindAsync(p => !p.IsDeleted);

            var result = new List<ProjectReportRowDto>();

            foreach (var project in projects.OrderBy(p => p.Name))
            {
                var members = await _unitOfWork.Repository<ProjectMember>()
                    .FindAsync(m => m.ProjectId == project.Id);

                var tasks = await _unitOfWork.Repository<ProjectTask>()
                    .FindAsync(t => t.ProjectId == project.Id && !t.IsDeleted);

                var taskList = tasks.ToList();
                var completedCount = taskList.Count(t => t.Status == Domain.Enums.TaskStatus.Done);
                var progress = taskList.Count == 0
                    ? 0
                    : (int)Math.Round(completedCount * 100.0 / taskList.Count);

                result.Add(new ProjectReportRowDto
                {
                    Name = project.Name,
                    Status = project.Status.ToString(),
                    MemberCount = members.Count(),
                    TaskCount = taskList.Count,
                    CompletedTaskCount = completedCount,
                    ProgressPercent = progress,
                    CreatedAt = project.CreatedAt
                });
            }

            return result;
        }

        private async Task<List<TaskReportRowDto>> BuildTaskReportRowsAsync(int projectId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.ProjectId == projectId && !t.IsDeleted);

            var result = new List<TaskReportRowDto>();

            foreach (var task in tasks.OrderBy(t => t.Title))
            {
                var assignments = await _unitOfWork.Repository<TaskAssignee>()
                    .FindAsync(a => a.TaskId == task.Id && !a.IsDeleted);

                var assigneeNames = new List<string>();
                foreach (var assignment in assignments)
                {
                    var users = await _unitOfWork.Repository<User>()
                        .FindAsync(u => u.Id == assignment.UserId);
                    var name = users.FirstOrDefault()?.FullName;
                    if (name != null) assigneeNames.Add(name);
                }

                var creators = await _unitOfWork.Repository<User>()
                    .FindAsync(u => u.Id == task.CreatedBy);

                result.Add(new TaskReportRowDto
                {
                    Title = task.Title,
                    Status = task.Status.ToString(),
                    Priority = task.Priority.ToString(),
                    Assignees = assigneeNames.Any() ? string.Join(", ", assigneeNames) : "Unassigned",
                    DueDate = task.DueDate?.ToString("yyyy-MM-dd"),
                    CreatedByName = creators.FirstOrDefault()?.FullName ?? "Unknown"
                });
            }

            return result;
        }
    }
}