using Xunit;
using FluentAssertions;
using Moq;
using TaskApp.Domain.Enums;
using TaskApp.Domain.Interfaces;
using TaskApp.Domain.Services;
using TaskEntity = TaskApp.Domain.Entities.Task;
using Status = TaskApp.Domain.Enums.TaskStatus;

namespace TaskApp.Tests;

public class TaskServiceUpdateTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Mock<IHolidayService> _mockHolidayService;
    private readonly TaskService _taskService;
    private readonly TaskEntity _validTask;

    public TaskServiceUpdateTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _mockHolidayService = new Mock<IHolidayService>();
        _taskService = new TaskService(_mockRepository.Object, _mockHolidayService.Object);

        // Existing task
        _validTask = new TaskEntity(
            "Original Task Name",
            "Original Description",
            DateTime.Now.Date.AddDays(-1),
            GetNextWeekday(3), 
            Priority.Medium
        )
        {
            Id = 101,
            Status = Status.New
        };

        _mockRepository.Setup(r => r.GetTaskByIdAsync(_validTask.Id)).ReturnsAsync(_validTask);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
    }

    #region Basic Update Functionality

    [Fact]
    public async Task UpdateTaskAsync_WithValidChanges_ShouldSucceedAndPersist()
    {
        var updatedTask = _validTask.DeepClone();
        updatedTask.Name = "New Name";
        updatedTask.Description = "New Description";

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        _mockRepository.Setup(r => r.UpdateTaskAsync(It.IsAny<TaskEntity>()));

        await _taskService.UpdateTaskAsync(updatedTask);

        // Call update method
        _mockRepository.Verify(r => r.UpdateTaskAsync(It.IsAny<TaskEntity>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

        _validTask.Name.Should().Be("New Name");
        _validTask.Description.Should().Be("New Description");
    }

    #endregion

    #region Requirement: Due Date Cannot Be On Weekend

    [Fact]
    public async Task UpdateTaskAsync_ChangingDueDateToSaturday_ShouldThrowArgumentException()
    {
        var saturday = GetNextSaturday();
        var updatedTask = _validTask.DeepClone();
        updatedTask.DueDate = saturday;

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);

        Func<Task> act = async () => await _taskService.UpdateTaskAsync(updatedTask);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Due date cannot fall on a weekend.");
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    #endregion

    #region Requirement: Due Date Cannot Be On Holiday

    [Fact]
    public async Task UpdateTaskAsync_ChangingDueDateToHoliday_ShouldThrowArgumentException()
    {
        var holidayDate = GetNextWeekday(7); // A future weekday
        var updatedTask = _validTask.DeepClone();
        updatedTask.DueDate = holidayDate; // Violates the rule

        _mockHolidayService.Setup(x => x.IsHoliday(holidayDate)).Returns(true);

        Func<Task> act = async () => await _taskService.UpdateTaskAsync(updatedTask);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Due date cannot be on a holiday.");
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    #endregion

    #region Requirement: Max 100 High Priority Tasks Per Due Date

    [Fact]
    public async Task UpdateTaskAsync_ChangeToHighPriority_WhenLimitReached_ShouldThrowInvalidOperationException()
    {
        var dueDate = _validTask.DueDate.Date;
        var updatedTask = _validTask.DeepClone();
        updatedTask.Priority = Priority.High; // Attempt to change to High

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        _mockRepository.Setup(r => r.HighPriorityTaskCountAsync(dueDate)).ReturnsAsync(100);

        Func<Task> act = async () => await _taskService.UpdateTaskAsync(updatedTask);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot add task. High priority task limit exceeded for the given due date.");
        _mockRepository.Verify(r => r.HighPriorityTaskCountAsync(dueDate), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskAsync_ChangeStatusToFinishedHighPriority_ShouldSucceed()
    {
        var highPriorityTask = _validTask.DeepClone();
        highPriorityTask.Priority = Priority.High;
        highPriorityTask.Status = Status.New;

        _mockRepository.Setup(r => r.GetTaskByIdAsync(highPriorityTask.Id)).ReturnsAsync(highPriorityTask);

        var updatedTask = highPriorityTask.DeepClone();
        updatedTask.Status = Status.Finished;

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        // Even if the limit is 100, finishing the task is always allowed
        _mockRepository.Setup(r => r.HighPriorityTaskCountAsync(It.IsAny<DateTime>())).ReturnsAsync(100);

        await _taskService.UpdateTaskAsync(updatedTask);

        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        highPriorityTask.Status.Should().Be(Status.Finished);
    }

    #endregion

    #region Helper Methods

    private DateTime GetNextWeekday(int daysAhead = 1)
    {
        var date = DateTime.Now.Date.AddDays(daysAhead);
        while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            date = date.AddDays(1);
        }
        return date;
    }

    private DateTime GetNextSaturday()
    {
        var date = DateTime.Now.Date.AddDays(1);
        while (date.DayOfWeek != DayOfWeek.Saturday)
        {
            date = date.AddDays(1);
        }
        return date;
    }
    #endregion

}

public static class TaskEntityExtensions
{
    public static TaskEntity DeepClone(this TaskEntity source)
    {
        return new TaskEntity(
            source.Name,
            source.Description,
            source.StartDate,
            source.DueDate,
            source.Priority
        )
        {
            Id = source.Id,
            Status = source.Status,
            EndDate = source.EndDate
        };
    }
}