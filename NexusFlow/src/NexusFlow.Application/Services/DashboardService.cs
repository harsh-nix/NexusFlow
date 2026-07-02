using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Dashboard;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Enums;
using NexusFlow.Domain.Interfaces;
using TaskStatus = NexusFlow.Domain.Enums.TaskStatus;

namespace NexusFlow.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DashboardStatsDto>> GetStatsAsync(int userId)
        {
            // Get user's projects
            var memberships = await _unitOfWork
                .Repository<ProjectMember>()
                .FindAsync(m => m.UserId == userId);

            var projectIds = memberships.Select(m => m.ProjectId).ToList();

            var projects = await _unitOfWork
                .Repository<Project>()
                .FindAsync(p => projectIds.Contains(p.Id) && !p.IsDeleted);

            var projectList = projects.ToList();

            // Get tasks for those projects
            var tasks = await _unitOfWork
                .Repository<ProjectTask>()
                .FindAsync(t => projectIds.Contains(t.ProjectId) && !t.IsDeleted);

            var taskList = tasks.ToList();

            // Get unread notifications
            var notifications = await _unitOfWork
                .Repository<Notification>()
                .FindAsync(n => n.UserId == userId && !n.IsRead);

            // Recent projects as activities
            var recentActivities = projectList
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new RecentActivityDto
                {
                    Title = p.Name,
                    Type = "Project",
                    Date = p.CreatedAt
                })
                .ToList();

            // Add recent tasks
            var recentTasks = taskList
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new RecentActivityDto
                {
                    Title = t.Title,
                    Type = "Task",
                    Date = t.CreatedAt
                })
                .ToList();

            recentActivities.AddRange(recentTasks);
            recentActivities = recentActivities
                .OrderByDescending(a => a.Date)
                .Take(5)
                .ToList();

            var stats = new DashboardStatsDto
            {
                TotalProjects = projectList.Count,
                ActiveProjects = projectList
                    .Count(p => p.Status == ProjectStatus.Active),
                TotalTasks = taskList.Count,
                CompletedTasks = taskList
                    .Count(t => t.Status == TaskStatus.Done),
                InProgressTasks = taskList
                    .Count(t => t.Status == TaskStatus.InProgress),
                PendingTasks = taskList
                    .Count(t => t.Status == TaskStatus.Todo),
                UnreadNotifications = notifications.Count(),
                RecentActivities = recentActivities
            };

            return ApiResponse<DashboardStatsDto>.Ok(stats);
        }
    }
}