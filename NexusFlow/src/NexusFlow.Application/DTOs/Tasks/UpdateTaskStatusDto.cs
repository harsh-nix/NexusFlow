using TaskStatus = NexusFlow.Domain.Enums.TaskStatus;

namespace NexusFlow.Application.DTOs.Tasks
{
    public class UpdateTaskStatusDto
    {
        public TaskStatus Status { get; set; }

        // Optional. When provided, this is saved as a WorkLog-typed comment
        // alongside the status change — e.g. "Finished backend, starting tests."
        public string? Note { get; set; }
    }
}