using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Domain.Interfaces;

public interface ITaskService
{
  public Task<int> AddTaskAsync(TaskEntity task);
  public Task<TaskEntity> UpdateTaskAsync(TaskEntity task);

  public Task<TaskEntity?> GetTaskByIdAsync(int id);
  public Task<IEnumerable<TaskEntity>> GetAllTasksAsync();
  public Task DeleteTaskAsync(int id);
}
