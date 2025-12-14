using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskApp.Domain.Interfaces;
using TaskEntity = TaskApp.Domain.Entities.Task;

namespace TaskApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskEntity>>> GetAllTasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return Ok(tasks);
        }
    }
}
