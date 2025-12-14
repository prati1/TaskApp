using TaskApp.Domain.Interfaces;
using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Domain.Services;

public class TaskService : ITaskService
{
  private readonly ITaskRepository _taskRepository;

  public TaskService(ITaskRepository taskRepository)
  {
      _taskRepository = taskRepository;
  }

  public async Task<int> AddTaskAsync(TaskEntity task) {
    // await _taskRepository.HighPriorityTaskCountAsync(DateTime.Now);

    var newTask = new TaskEntity(
        task.Name,
        task.Description,
        task.StartDate,
        task.DueDate,
        task.Priority
    );

    await _taskRepository.AddTaskAsync(newTask);
    await _taskRepository.SaveChangesAsync();
    return newTask.Id;
  }

  public async Task<TaskEntity> UpdateTaskAsync(TaskEntity task) {
      var existingTask = await _taskRepository.GetTaskByIdAsync(task.Id);
      if (existingTask == null) {
          throw new KeyNotFoundException($"Task {task.Name} with ID {task.Id} not found");
      }

      existingTask.Update(
          task.Name,
          task.Description,
          task.DueDate,
          task.Priority,
          task.Status,
          task.StartDate
      );

      _taskRepository.UpdateTaskAsync(existingTask);
      await _taskRepository.SaveChangesAsync();
      return existingTask;
  }

  public async Task DeleteTaskAsync(int id) {
      var existingTask = await _taskRepository.GetTaskByIdAsync(id);
      if (existingTask == null) {
          throw new KeyNotFoundException($"Task with ID {id} not found");
      }

      _taskRepository.DeleteTaskAsync(existingTask);
      await _taskRepository.SaveChangesAsync();
  }

  public async Task<TaskEntity?> GetTaskByIdAsync(int id)
  {
      return await _taskRepository.GetTaskByIdAsync(id);
  }

  public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync()
  {
      return await _taskRepository.GetAllTasksAsync();
  }
}
