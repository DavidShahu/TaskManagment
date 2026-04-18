using Application.Tasks.DTOs;
using Application.Tasks.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagment.Extencions;

namespace TaskManagment.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            ITaskService taskService,
            ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        // GET api/tasks — Admin gets all, User gets own
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var tasks = isAdmin
                ? await _taskService.GetAllAsync(cancellationToken)
                : await _taskService.GetMyTasksAsync(userId, cancellationToken);

            return Ok(tasks);
        }

        // GET api/tasks/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTasks(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var tasks = await _taskService
                .GetMyTasksAsync(userId, cancellationToken);
            return Ok(tasks);
        }

        // GET api/tasks/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var task = await _taskService.GetByIdAsync(id, cancellationToken);
            return Ok(task);
        }

        // GET api/tasks/project/{projectId}
        [HttpGet("project/{projectId:guid}")]
        public async Task<IActionResult> GetByProject(Guid projectId, CancellationToken cancellationToken)
        {
            var tasks = await _taskService
                .GetByProjectAsync(projectId, cancellationToken);
            return Ok(tasks);
        }

        // POST api/tasks
        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            // Only admin can assign to other users
            if (request.AssignedToUserId.HasValue && !isAdmin)
                return Forbid();

            _logger.LogInformation(
                "Creating task {Title} by user {UserId}",
                request.Title, userId);

            var task = await _taskService
                .CreateAsync(request, userId, cancellationToken);

            return CreatedAtAction(
                nameof(GetById), new { id = task.Id }, task);
        }

        // PUT api/tasks/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            var task = await _taskService
                .UpdateAsync(id, request, userId, isAdmin, cancellationToken);

            return Ok(task);
        }

        // DELETE api/tasks/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            await _taskService
                .DeleteAsync(id, userId, isAdmin, cancellationToken);

            return NoContent();
        }

        // PATCH api/tasks/{id}/done
        [HttpPatch("{id:guid}/done")]
        public async Task<IActionResult> MarkAsDone(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            await _taskService
                .MarkAsDoneAsync(id, userId, isAdmin, cancellationToken);

            return Ok(new { message = "Task marked as done" });
        }

        // PATCH api/tasks/{id}/open
        [HttpPatch("{id:guid}/open")]
        public async Task<IActionResult> MarkAsOpen(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            await _taskService
                .MarkAsOpenAsync(id, userId, isAdmin, cancellationToken);

            return Ok(new { message = "Task marked as open" });
        }

        // POST api/tasks/{id}/log-time
        [HttpPost("{id:guid}/log-time")]
        public async Task<IActionResult> LogTime(Guid id, LogTimeRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();

            _logger.LogInformation(
                "User {UserId} logging {Hours} hours for task {TaskId}",
                userId, request.Hours, id);

            var task = await _taskService
                .LogTimeAsync(id, userId, request, cancellationToken);

            return Ok(task);
        }



        // GET api/tasks/timer
        [HttpGet("timer")]
        public async Task<IActionResult> GetActiveTimer(
            CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var timer = await _taskService
                .GetActiveTimerAsync(userId, cancellationToken);
            return Ok(timer);
        }

        // POST api/tasks/{id}/timer/start
        [HttpPost("{id:guid}/timer/start")]
        public async Task<IActionResult> StartTimer(Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            _logger.LogInformation(
                "User {UserId} starting timer for task {TaskId}", userId, id);
            var timer = await _taskService
                .StartTimerAsync(id, userId, cancellationToken);
            return Ok(timer);
        }

        // POST api/tasks/timer/stop
        [HttpPost("timer/stop")]
        public async Task<IActionResult> StopTimer(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            _logger.LogInformation("User {UserId} stopping timer", userId);
            var hours = await _taskService
                .StopTimerAsync(userId, cancellationToken);
            return Ok(new { hours });
        }
    }
}
