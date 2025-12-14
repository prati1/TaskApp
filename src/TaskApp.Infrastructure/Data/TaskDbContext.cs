using Microsoft.EntityFrameworkCore;
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
    }

}
