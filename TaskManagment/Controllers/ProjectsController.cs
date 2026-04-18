using Application.Projects.DTOs.ProjectDTOs;
using Application.Projects.Interfaces.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagment.Extencions;

namespace TaskManagment.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            IProjectService projectService,
            ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }



        // GET api/projects — Admin gets all, User gets their own
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "Getting projects for user {UserId}, isAdmin: {IsAdmin}",
                userId, isAdmin);

            // if it is admin get all project (even the ones that are deactivated),  if its a normal user just get the ones they are part of 
            var projects = isAdmin
                ? await _projectService.GetAllAsync(includeDeactivated: true, cancellationToken)
                : await _projectService.GetByUserAsync(userId, cancellationToken);

            return Ok(projects);
        }

        // GET api/projects/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var project = await _projectService.GetByIdAsync(id, cancellationToken);
            return Ok(project);
        }

        // POST api/projects — Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateProjectRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();

            _logger.LogInformation(
                "Creating project {Name} by user {UserId}",
                request.Name, userId);

            var project = await _projectService
                .CreateAsync(request, userId, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }

        // PUT api/projects/{id} — Admin only
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, UpdateProjectRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating project {ProjectId}", id);
            var project = await _projectService
                .UpdateAsync(id, request, cancellationToken);
            return Ok(project);
        }

        // DELETE api/projects/{id} — Admin only
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting project {ProjectId}", id);
            await _projectService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        // POST api/projects/{id}/members/{userId} — Admin only
        [HttpPost("{id:guid}/members/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMember(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Adding user {UserId} to project {ProjectId}",
                userId, id);

            await _projectService.AddMemberAsync(id, userId, cancellationToken);
            return Ok(new { message = "Member added successfully" });
        }

        // DELETE api/projects/{id}/members/{userId} — Admin only
        [HttpDelete("{id:guid}/members/{userId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Removing user {UserId} from project {ProjectId}",
                userId, id);

            await _projectService.RemoveMemberAsync(id, userId, cancellationToken);
            return Ok(new { message = "Member removed successfully" });
        }

        // PATCH api/projects/{id}/activate — Admin only
        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Activating project {ProjectId}", id);
            await _projectService.ActivateAsync(id, cancellationToken);
            return Ok(new { message = "Project activated successfully" });
        }
    }
}
