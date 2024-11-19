using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartMenuManagerApp.Authentication;
using SmartMenuManagerApp.Dto;

namespace SmartMenuManagerApp.Controllers
{
    [Route("api/auth")]
    [ApiController] // Ensures that this controller responds to HTTP requests
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; // Dependency injection for AuthService
        private readonly ILogger<AuthController> _logger;

        // Constructor to initialize AuthService
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Null check to ensure logger is injected
        }

        // Endpoint for user registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Log when a registration request comes in
                _logger.LogInformation("Registering user: {UserEmail}", registerDto.Email);
                // Call AuthService to register the user and get the JWT token
                var token = await _authService.RegisterAsync(registerDto);
                return Ok(new { Token = token }); // Return token in response
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"Error during registration: {ex.Message}", ex);

                // Return a 500 status code for server-side errors
                return StatusCode(500, new { message = "An error occurred during registration.", details = ex.Message });
            }
        }

        // Endpoint for user login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Call AuthService to log in and get the JWT token
                var token = await _authService.LoginAsync(loginDto);
                return Ok(new { Token = token }); // Return token in response
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message }); // Return unauthorized if login fails
            }
        }
    }
}
