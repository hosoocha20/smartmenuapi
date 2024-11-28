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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IJwtService _jwtService;

        public ProductController(IProductService productService, IJwtService jwtService)
        {
            _productService = productService;
            _jwtService = jwtService;
        }

        [HttpPost("create")]
        [Authorize(Policy = "Jwt_Or_Identity")] // Ensures only authenticated users can create a menu category
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto request)
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

                
                var product = await _productService.AddProductAsync(request, userId);
                return CreatedAtAction(nameof(CreateProduct), new { id = product.Id }, product);
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

        [Authorize(Policy = "AdminOnly")]
        [Authorize(Policy = "Jwt_Or_Identity")]
        [HttpGet("admin-data")]
        public IActionResult GetAdminData()
        {
            return Ok(new { message = "This is admin-only data" });
        }
    }
}
