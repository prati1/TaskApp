using TaskApp.Domain.Enums;

namespace TaskApp.Api.DTOs;

public record AddTaskRequest(
    string Name,
    string? Description,
    DateTime DueDate,
    DateTime StartDate,
    Priority Priority
);
