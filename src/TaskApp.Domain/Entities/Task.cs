using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskApp.Domain.Enums;

namespace TaskApp.Domain.Entities;

public class Task
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }
    public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.New;
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime? EndDate { get; set; }

    private Task() { }

    public Task(
        string name,
        string? description,
        DateTime? startDate,
        DateTime dueDate,
        Priority priority)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        DueDate = dueDate;
        Priority = priority;
        Status = Enums.TaskStatus.New;
    }

    public void Update(
    string name,
    string? description,
    DateTime dueDate,
    Priority priority,
    Enums.TaskStatus status,
    DateTime? startDate
    )
    {
        Name = name;
        DueDate = dueDate;
        Priority = priority;
        if (startDate is not null) StartDate = startDate;
        if (description is not null) Description = description;

        // Set EndDate if status is changed to Finished
        if (status == Enums.TaskStatus.Finished && Status != Enums.TaskStatus.Finished)
        {
            EndDate = DateTime.UtcNow;
        }

        // Set StartDate to now if status is changes from New to InProgress and StartDate is null
        if (startDate is null && status == Enums.TaskStatus.InProgress && Status == Enums.TaskStatus.New)
        {
            StartDate = DateTime.UtcNow;
        }

        Status = status;
    }
}
