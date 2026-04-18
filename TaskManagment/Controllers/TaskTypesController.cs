using Application.TaskTypes.DTOs;
using Application.TaskTypes.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagment.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskTypesController : ControllerBase
    {
        private readonly ITaskTypeService _taskTypeService;

        public TaskTypesController(ITaskTypeService taskTypeService)
        {
            _taskTypeService = taskTypeService;
        }

        // GET api/tasktypes — everyone can get types
        [HttpGet]
        public async Task<IActionResult> GetAll( CancellationToken cancellationToken)
        {
            var types = await _taskTypeService.GetAllAsync(cancellationToken);
            return Ok(types);
        }

        // POST api/tasktypes — admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create( CreateTaskTypeRequest request, CancellationToken cancellationToken)
        {
            var type = await _taskTypeService
                .CreateAsync(request, cancellationToken);
            return Ok(type);
        }

        // PUT api/tasktypes/{id} — admin only
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update( Guid id, UpdateTaskTypeRequest request, CancellationToken cancellationToken)
        {
            var type = await _taskTypeService
                .UpdateAsync(id, request, cancellationToken);
            return Ok(type);
        }

        // DELETE api/tasktypes/{id} — admin only
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete( Guid id, CancellationToken cancellationToken)
        {
            await _taskTypeService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH api/tasktypes/{id}/activate — admin only
        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(  Guid id, CancellationToken cancellationToken)
        {
            await _taskTypeService.ActivateAsync(id, cancellationToken);
            return Ok(new { message = "Task type activated" });
        }
    }
}
