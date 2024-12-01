﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IMenuCategoryService _menuCategoryService;
        private readonly IJwtService _jwtService; // Inject JwtService to validate token

        public MenuCategoryController(IMenuCategoryService menuCategoryService, IJwtService jwtService)
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

        [HttpDelete("category/{categoryId}")]
        [Authorize(Policy = "Jwt_Or_Identity")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
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
                var result = await _menuCategoryService.DeleteCategoryAsync(categoryId, int.Parse(restaurantId));
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

        [HttpGet("category/getTable")]
        [Authorize(Policy = "Jwt_Or_Identity")]
        public async Task<IActionResult> GetUserTableCategories()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
 
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }

                // Pass the user ID to the service to check authorization
                var menuCategories = await _menuCategoryService.GetAllTableMenuCategoriesAsync(userId);
                if (menuCategories == null || !menuCategories.Any())
                {
                    return NotFound(new { message = "No sub categories found or access denied." });
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

        [HttpPost("subcategory/create")]
        [Authorize(Policy = "Jwt_Or_Identity")] // Ensures only authenticated users can create a menu category
        public async Task<IActionResult> CreateSubCategory([FromBody] CreateMenuSubCategoryDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Extract the user ID from the JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }

                // Pass the user ID to the service to check authorization
                var menuSubCategory = await _menuCategoryService.CreateSubCategoryAsync(request, userId);
                return CreatedAtAction(nameof(CreateSubCategory), new { id = menuSubCategory.Id }, menuSubCategory);
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

        [HttpDelete("subcategory/{subCategoryId}")]
        [Authorize(Policy = "Jwt_Or_Identity")]
        public async Task<IActionResult> DeleteSubCategory(int subCategoryId)
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
                var result = await _menuCategoryService.DeleteSubCategoryAsync(subCategoryId, int.Parse(restaurantId));
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

        [HttpGet("allsubcategories")]
        [Authorize(Policy = "Jwt_Or_Identity")] // Ensures only authenticated users can create a menu category
        public async Task<IActionResult> GetAllMenuSubCategories()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid or expired token." });
                }
                var menuSubCategories = await _menuCategoryService.GetAllSubCategoryAsync(userId);
                if (menuSubCategories == null || !menuSubCategories.Any())
                {
                    return NotFound(new { message = "No sub categories found or access denied." });
                }

                return Ok(menuSubCategories);
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

        [HttpGet("menu")]
        [Authorize(Policy = "Jwt_Or_Identity")] // Ensures only authenticated users can get menu
        public async Task<IActionResult> GetUserMenu()
        {
            try
            {
                // Extract the user ID from the JWT token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User is not authenticated." });

                // Call the service to get the menu
                var menu = await _menuCategoryService.GetUserMenuAsync(userId);

                if (menu == null)
                    return NotFound(new { message = "No menu found for this user." });

                return Ok(menu);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }
    }


 
}

