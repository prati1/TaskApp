using Microsoft.EntityFrameworkCore;
using TaskApp.Domain.Interfaces;
using TaskApp.Infrastructure.Data;
using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;
    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(int id)
    {
        return await _context.Tasks.Where(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task AddTaskAsync(TaskEntity task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public void UpdateTaskAsync(TaskEntity task)
    {
        _context.Tasks.Update(task);
    }

    public void DeleteTaskAsync(TaskEntity task)
    {
        _context.Tasks.Remove(task);
    }

    public async Task<int> HighPriorityTaskCountAsync(DateTime dueDate)
    {
        throw new NotImplementedException();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
