using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartMenuManagerApp.Authentication;
using SmartMenuManagerApp.Services;
using System.Security.Claims;

namespace SmartMenuManagerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IJwtService jwtService, IRestaurantService restaurantService)
        {
            _jwtService = jwtService;
            _restaurantService = restaurantService;
        }

        [HttpGet("get")]
        [Authorize(Policy = "Jwt_Or_Identity")]
        public async Task<IActionResult> GetRestaurant()
        {
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Console.WriteLine("Controller" + userId);
                    if (string.IsNullOrEmpty(userId))
                    {
                        return Unauthorized(new { message = "Invalid or expired token." });
                    }
                    var restaurant = await _restaurantService.GetRestaurantAsync(userId);
                    if (restaurant == null)
                    {
                        return NotFound(new { message = "No restaruant found or access denied." });
                    }

                    return Ok(restaurant);
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
    }
}
