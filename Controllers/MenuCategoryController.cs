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
    public class MenuCategoryController : ControllerBase
    {
        private readonly MenuCategoryService _menuCategoryService;
        private readonly IJwtService _jwtService; // Inject JwtService to validate token

        public MenuCategoryController(MenuCategoryService menuCategoryService, IJwtService jwtService)
        {
            _menuCategoryService = menuCategoryService;
            _jwtService = jwtService;
        }

        [HttpPost("category/create")]
        [Authorize(Policy = "Jwt_Or_Identity")] // Ensures only authenticated users can create a menu category
        public async Task<IActionResult> CreateCategory([FromBody] CreateMenuCategoryDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Get the token from the Authorization header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // Validate the token and get the user's claims
                var principal = _jwtService.ValidateToken(token);
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from the token
                Console.WriteLine("Controller" + userId);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }

                // Pass the user ID to the service to check authorization
                var menuCategory = await _menuCategoryService.CreateCategoryAsync(request, userId);
                return CreatedAtAction(nameof(CreateCategory), new { id = menuCategory.Id }, menuCategory);
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

        [HttpPost("categories/{userId}")]
        [Authorize(Policy = "Jwt_Or_Identity")] // Ensures only authenticated users can create a menu category
        public async Task<IActionResult> GetMenuCategories(string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var menuCategories = await _menuCategoryService.GetUserMenuCategoriesAsync(userId);
                if (menuCategories == null || !menuCategories.Any())
                {
                    return NotFound(new { message = "No categories found or access denied." });
                }

                return Ok(menuCategories);
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
    }
    /*        private readonly MenuCategoryService _menuCategoryService;

            public MenuCategoryController(MenuCategoryService menuCategoryService)
            {
                _menuCategoryService = menuCategoryService;
            }

            [HttpPost("category/create")]
            //[Authorize] // Ensures only authenticated users can create a menu category
            public async Task<IActionResult> CreateCategory([FromBody] CreateMenuCategoryDto request)
            {
                try
                {
                    var menuCategory = await _menuCategoryService.CreateCategoryAsync(request);
                    return CreatedAtAction(nameof(CreateCategory), new { id = menuCategory.Id }, menuCategory);
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
            }*/

 
}

