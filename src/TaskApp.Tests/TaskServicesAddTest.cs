using Xunit;
using FluentAssertions;
using Moq;
using TaskApp.Domain.Enums;
using TaskApp.Domain.Interfaces;
using TaskApp.Domain.Services;
using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Tests;

public class TaskServiceAddTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly Mock<IHolidayService> _mockHolidayService;
    private readonly TaskService _taskService;

    public TaskServiceAddTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _mockHolidayService = new Mock<IHolidayService>();
        _taskService = new TaskService(_mockRepository.Object, _mockHolidayService.Object);
    }

    #region Happy Path

    [Fact]
    public async Task AddTaskAsync_HappyPath()
    {
        var today = DateTime.Today;
        var task = new TaskEntity(
            "Basic Task",
            "This is a basic task",
            today,
            today.AddDays(2),
            Priority.Low
        );

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        _mockRepository.Setup(x => x.AddTaskAsync(It.IsAny<TaskEntity>())).Returns(Task.CompletedTask);
        _mockRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _taskService.AddTaskAsync(task);

        result.Should().Be(task.Id);
        _mockRepository.Verify(x => x.AddTaskAsync(task), Times.Once);
    }

    #endregion

    #region Requirement:  Due Date Cannot Be In The Past

    [Fact]
    public void AddTaskAsync_WithPastDueDate_ShouldThrowArgumentException()
    {
        var pastDate = DateTime.Now.AddDays(-1);
        var task = new TaskEntity(
            "Past Task",
            "This task has a past due date",
            DateTime.Now.AddDays(-2),
            pastDate,
            Priority.Low
        );

        Func<Task> act = async () => await _taskService.AddTaskAsync(task);

        act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Due date cannot be in the past.");
    }

    [Fact]
    public async Task AddTaskAsync_WithFutureDueDate_ShouldAddSuccessfully()
    {
        var futureWeekday = GetNextWeekday(7);
        var task = new TaskEntity(
            "Future Task",
            "Due in the future",
            futureWeekday.AddDays(-2),
            futureWeekday,
            Priority.Medium
        );

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        _mockRepository.Setup(x => x.AddTaskAsync(It.IsAny<TaskEntity>())).Returns(Task.CompletedTask);
        _mockRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _taskService.AddTaskAsync(task);

        result.Should().Be(task.Id);
        _mockRepository.Verify(x => x.AddTaskAsync(task), Times.Once);
    }

    #endregion

    #region Requirement:  Due Date Cannot Be On Weekend

    [Fact]
    public async Task AddTaskAsync_WithWeekendDueDate_ShouldThrowArgumentException()
    {
        var saturday = new DateTime(2025, 12, 27);
        var task = new TaskEntity(
            "Weekend Task",
            "Due on Weekend",
            saturday.AddDays(-2),
            saturday,
            Priority.Low
        );

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);

        Func<Task> act = async () => await _taskService.AddTaskAsync(task);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Due date cannot fall on a weekend.");
    }

    [Theory]
    [InlineData(DayOfWeek.Wednesday)]
    public async Task AddTaskAsync_WithWeekdayDueDate_ShouldAddSuccessfully(DayOfWeek dayOfWeek)
    {
        var weekday = GetNextDayOfWeek(dayOfWeek);
        var task = new TaskEntity(
            $"{dayOfWeek} Task",
            $"Due on {dayOfWeek}",
            weekday.AddDays(-2),
            weekday,
            Priority.Low
        );

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        _mockRepository.Setup(x => x.AddTaskAsync(It.IsAny<TaskEntity>())).Returns(Task.CompletedTask);
        _mockRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _taskService.AddTaskAsync(task);

        result.Should().Be(task.Id);
        _mockRepository.Verify(x => x.AddTaskAsync(task), Times.Once);
    }

    #endregion

    #region Requirement: Due Date Cannot Be On Holiday

    [Fact]
    public async Task AddTaskAsync_WithHolidayDueDate_ShouldThrowArgumentException()
    {
        var holiday = GetNextWeekday(7);
        var task = new TaskEntity(
            "Holiday Task",
            "Due on a holiday",
            holiday.AddDays(-2),
            holiday,
            Priority.Medium
        );

        _mockHolidayService.Setup(x => x.IsHoliday(holiday)).Returns(true);

        Func<Task> act = async () => await _taskService.AddTaskAsync(task);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Due date cannot be on a holiday.");
        _mockHolidayService.Verify(x => x.IsHoliday(holiday), Times.Once);
    }
    #endregion

    #region Requirement: Max 100 High Priority Tasks Per Due Date for tasks that aren't finished

    [Fact]
    public async Task AddTaskAsync_HighPriority_WhenLimitNotExceeded_ShouldAddSuccessfully()
    {
        var dueDate = GetNextWeekday(7);
        var task = new TaskEntity(
            "High Priority Task",
            "Important task",
            dueDate.AddDays(-2),
            dueDate,
            Priority.High
        );

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        _mockRepository.Setup(x => x.HighPriorityTaskCountAsync(dueDate)).ReturnsAsync(99); // 99 existing
        _mockRepository.Setup(x => x.AddTaskAsync(It.IsAny<TaskEntity>())).Returns(Task.CompletedTask);
        _mockRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _taskService.AddTaskAsync(task);

        result.Should().Be(task.Id);
        _mockRepository.Verify(x => x.HighPriorityTaskCountAsync(dueDate), Times.Once);
        _mockRepository.Verify(x => x.AddTaskAsync(task), Times.Once);
    }

    [Fact]
    public async Task AddTaskAsync_HighPriority_WhenLimitReached_ShouldThrowInvalidOperationException()
    {
        var dueDate = GetNextWeekday(7);
        var task = new TaskEntity(
            "High Priority Task",
            "This would be the 101st task",
            dueDate.AddDays(-2),
            dueDate,
            Priority.High
        );

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        _mockRepository.Setup(x => x.HighPriorityTaskCountAsync(dueDate)).ReturnsAsync(100); // Limit reached

        Func<Task> act = async () => await _taskService.AddTaskAsync(task);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot add task. High priority task limit exceeded for the given due date.");
        _mockRepository.Verify(x => x.HighPriorityTaskCountAsync(dueDate), Times.Once);
        _mockRepository.Verify(x => x.AddTaskAsync(It.IsAny<TaskEntity>()), Times.Never);
    }

    [Fact]
    public async Task AddTaskAsync_MediumPriority_ShouldNotCheckLimit()
    {
        var dueDate = GetNextWeekday(7);
        var task = new TaskEntity(
            "Medium Priority Task",
            "Medium priority tasks have no limit",
            dueDate.AddDays(-2),
            dueDate,
            Priority.Medium
        );

        _mockHolidayService.Setup(x => x.IsHoliday(It.IsAny<DateTime>())).Returns(false);
        _mockRepository.Setup(x => x.AddTaskAsync(It.IsAny<TaskEntity>())).Returns(Task.CompletedTask);
        _mockRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _taskService.AddTaskAsync(task);

        result.Should().Be(task.Id);
        _mockRepository.Verify(x => x.HighPriorityTaskCountAsync(It.IsAny<DateTime>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private DateTime GetNextWeekday(int daysAhead = 1)
    {
        var date = DateTime.Now.AddDays(daysAhead);
        while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            date = date.AddDays(1);
        }
        return date;
    }

    private DateTime GetNextDayOfWeek(DayOfWeek targetDay)
    {
        var date = DateTime.Now.AddDays(1);
        while (date.DayOfWeek != targetDay)
        {
            date = date.AddDays(1);
        }
        return date;
    }

    #endregion
}