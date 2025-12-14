using TaskApp.Domain.Interfaces;
using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Domain.Services;

public class TaskService : ITaskService
{
  private readonly ITaskRepository _taskRepository;
  private readonly IHolidayService _holidayService;

  public TaskService(ITaskRepository taskRepository, IHolidayService holidayService)
  {
      _taskRepository = taskRepository;
      _holidayService = holidayService;
  }

  public async Task<int> AddTaskAsync(TaskEntity task) {
    ValidatePastDueDate(task);
    ValidateDueDateRules(task);
    await ValidateHighPriorityTaskLimit(task);

    await _taskRepository.AddTaskAsync(task);
    await _taskRepository.SaveChangesAsync();
    return task.Id;
  }

  public async Task<TaskEntity> UpdateTaskAsync(TaskEntity task) {
      var existingTask = await _taskRepository.GetTaskByIdAsync(task.Id);
      if (existingTask == null) {
          throw new KeyNotFoundException($"Task {task.Name} with ID {task.Id} not found");
      }

      ValidateDueDateRules(task);
      if (existingTask.Priority != task.Priority && task.Status != Enums.TaskStatus.Finished) {
        await ValidateHighPriorityTaskLimit(task);
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

  private async Task ValidateHighPriorityTaskLimit(TaskEntity task)
  {
      if (task.Priority == Enums.Priority.High && task.Status != Enums.TaskStatus.Finished)
      {
        int highPriorityTaskCount = await _taskRepository.HighPriorityTaskCountAsync(task.DueDate);
        if (highPriorityTaskCount >= 100)
        {
            throw new InvalidOperationException("Cannot add task. High priority task limit exceeded for the given due date.");
        }
      }
  }

  private void ValidateDueDateRules(TaskEntity task)
  {
    // Users might start a task only after its due date, so this validation is removed.
    //   if (task.StartDate != null && task.StartDate.Value.Date > task.DueDate.Date)
    //   {
    //       throw new ArgumentException("Start date cannot be after due date.");
    //   }

      if (task.DueDate.DayOfWeek == DayOfWeek.Saturday || task.DueDate.DayOfWeek == DayOfWeek.Sunday)
      {
          throw new ArgumentException("Due date cannot fall on a weekend.");
      }

      if (_holidayService.IsHoliday(task.DueDate))
      {
          throw new ArgumentException("Due date cannot be on a holiday.");
      }
  }

  private void ValidatePastDueDate(TaskEntity task)
  {
      if (task.DueDate.Date < DateTime.Now.Date)
      {
          throw new ArgumentException("Due date cannot be in the past.");
      }
  }

}
