using Microsoft.AspNetCore.Mvc;
using TaskApp.Api.DTOs;
using TaskApp.Domain.Services;
using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskEntity>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskEntity>> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult> AddTask([FromBody] AddTaskRequest task)
        {
            if (task is null)
            {
                return BadRequest();
            }
            var taskEntity = new TaskEntity(
                task.Name,
                task.Description,
                task.DueDate,
                task.StartDate,
                task.Priority
            );
            await _taskService.AddTaskAsync(taskEntity);
            return CreatedAtAction(nameof(GetTaskById), new { id = taskEntity.Id }, taskEntity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequest task)
        {
            var taskEntity = new TaskEntity(
                task.Name,
                task.Description,
                task.DueDate,
                task.StartDate,
                task.Priority
            )
            {
                Id = id,
                Status = task.Status,
            };

            var result = await _taskService.UpdateTaskAsync(taskEntity);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}