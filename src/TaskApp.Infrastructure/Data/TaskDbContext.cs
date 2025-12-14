using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Enums;
using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Infrastructure.Data;

public class TaskDbContext : DbContext
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options) { }

    public DbSet<TaskEntity> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ...existing code...
        modelBuilder.Entity<TaskEntity>().HasData(
            new TaskEntity(
                name: "Complete Project Documentation",
                description: "Complete the documentation for the project, including all modules and APIs.",
                startDate: new DateTime(2025, 12, 18),
                dueDate: new DateTime(2025, 12, 21),
                priority: Priority.High
            )
            {
                Id = 1,
                Status = Domain.Enums.TaskStatus.InProgress
            },
            new TaskEntity(
                name: "Onboarding New Developers",
                description: "Prepare onboarding materials and sessions for new developers joining the team.",
                startDate: null,
                dueDate: new DateTime(2025, 12, 18),
                priority: Priority.Medium
            )
            {
                Id = 2,
                Status = Domain.Enums.TaskStatus.New
            },
            new TaskEntity(
                name: "Finished: Update Documentation",
                description: "Updated the API documentation for version 2.0.",
                startDate: null,
                dueDate: new DateTime(2025, 12, 10),
                priority: Priority.Low
            )
            {
                Id = 3,
                EndDate = new DateTime(2025, 12, 10),
                Status = Domain.Enums.TaskStatus.New
            }
        );
    }

}
