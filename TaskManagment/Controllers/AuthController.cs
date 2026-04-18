using Application.Auth.DTOs;
using Application.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Register attempt for email: {Email}", request.Email);
            var response = await _authService.RegisterAsync(request, cancellationToken);

            _logger.LogInformation("User registered successfully: {Email}", request.Email);
            return Ok(response);

            //middleware handles exeptions now
            //catch (InvalidOperationException ex)
            //{
            //    return Conflict(new { message = ex.Message });
            //}
            //catch (ArgumentException ex)
            //{

            //    return BadRequest(new { message = ex.Message });
            //}
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {

            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);

        }

    }
}
