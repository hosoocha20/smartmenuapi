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

                
                var createdProduct = await _productService.AddProductAsync(request, userId);
                var productResponse = new ProductResponseDto
                {
                    Id = createdProduct.Id,
                    Name = createdProduct.Name,
                    Description = createdProduct.Description,
                    Price = createdProduct.Price,
                    ImgUrl = createdProduct.ImgUrl,
                    MenuCategoryId = createdProduct.MenuCategoryId,
                    MenuSubCategoryId = createdProduct.MenuSubCategoryId,
                    Options = createdProduct.ProductOptions.Select(po => new ProductOptionDto
                    {
                        Id = po.Id,
                        Name = po.Name,
                        OptionDetails = po.OptionDetails.Select(d => new OptionDetailDto
                        {
                            Id = d.Id,
                            Name = d.Name,
                            AdditionalPrice = d.AdditionalPrice
                        }).ToList()
                    }).ToList(),
                    Labels = createdProduct.ProductLabels.Select(pl => new LabelDto
                    {
                        Id = pl.Label.Id,
                        Name = pl.Label.Name
                    }).ToList()
                };

                return CreatedAtAction(nameof(CreateProduct), new { id = createdProduct.Id }, productResponse);
                //return Ok("Product created successfully.");
                //return CreatedAtAction(nameof(CreateProduct), new { id = product.Id }, product);
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

        [HttpDelete("{productId}")]
        [Authorize(Policy = "Jwt_Or_Identity")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }
                var restaurantId = User.FindFirst("restaurantId")?.Value;

                if (string.IsNullOrEmpty(restaurantId))
                {
                    return Unauthorized("Restaurant ID not found in the token.");
                }

                // Pass the user ID to the service to check authorization
                var result = await _productService.DeleteProductAsync(productId, int.Parse(restaurantId));
                // Check the result status and return the appropriate HTTP response
                if (result.Status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
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
