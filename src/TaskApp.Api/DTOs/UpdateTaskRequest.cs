using TaskStatus = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Api.DTOs;

public record UpdateTaskRequest : AddTaskRequest
{
    public UpdateTaskRequest(string Name, string? Description, DateTime DueDate, DateTime? StartDate, Domain.Enums.Priority Priority) : base(Name, Description, DueDate, StartDate, Priority)
    {
    }

    public TaskStatus Status { get; set; }
}
