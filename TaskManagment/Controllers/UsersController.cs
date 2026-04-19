using Application.Users.DTOs;
using Application.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagment.Extencions;

namespace TaskManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : Controller
    {

        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        // GET api/users — Admin only
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin {UserId} fetching all users",
                User.GetUserId());

            var users = await _userService.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        // GET api/users/{id}
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            return Ok(user);
        }

        // DELETE api/users/{id} — Admin only
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin {AdminId} deleting user {UserId}",
                User.GetUserId(), id);

            await _userService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        // PATCH api/users/{id}/status — Admin only
        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Admin {AdminId} updating status for user {UserId} to {Status}",
                User.GetUserId(), id, request.IsActive);

            await _userService.UpdateStatusAsync(id, request.IsActive, cancellationToken);
            return NoContent();
        }

        // POST api/users/{id}/reset-password — Admin only
        [HttpPost("{id:guid}/reset-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] AdminResetPasswordRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Admin {AdminId} resetting password for user {UserId}",
                User.GetUserId(), id);

            await _userService.AdminResetPasswordAsync(
                id, request.NewPassword, cancellationToken);

            return Ok(new { message = "Password reset successfully" });
        }

    }
}

public record UpdateStatusRequest(bool IsActive);