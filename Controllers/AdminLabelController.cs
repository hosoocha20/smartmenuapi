using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenuManagerApp.Authentication;
using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Services;
using System.Security.Claims;

namespace SmartMenuManagerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminLabelController :ControllerBase
    {
        private readonly IAdminLabelService _adminLabelService;
        private readonly IJwtService _jwtService; // Inject JwtService to validate token

        public AdminLabelController(IAdminLabelService adminLabelService, IJwtService jwtService)
        {
            _adminLabelService = adminLabelService;
            _jwtService = jwtService;
        }

        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "Jwt_Or_Identity")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateLabel([FromBody] CreateLabelDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Extract the user ID from the JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //Console.WriteLine("Controller" + userId);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }


                var label = await _adminLabelService.AddLabelAsync(request);
                return CreatedAtAction(nameof(CreateLabel), new { id = label.Id }, label);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Jwt_Or_Identity")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteLabel(int id)
        {
            try
            {
                // Extract the user ID from the JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //Console.WriteLine("Controller" + userId);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }

                await _adminLabelService.DeleteLabelAsync(id);
                return NoContent(); // 204 successful deletion
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }
    }
}
