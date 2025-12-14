using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Domain.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllTasksAsync();
    Task<TaskEntity?> GetTaskByIdAsync(int id);
    Task AddTaskAsync(TaskEntity task);
    void UpdateTaskAsync(TaskEntity task);
    void DeleteTaskAsync(TaskEntity task);
    Task<int> HighPriorityTaskCountAsync(DateTime dueDate);
    Task SaveChangesAsync();
}
