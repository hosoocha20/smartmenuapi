using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;
using System.Security.Claims;

namespace SmartMenuManagerApp.Services
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MenuCategoryService(IMenuCategoryRepository menuCategoryRepository,
                                    IRestaurantRepository restaurantRepository,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _menuCategoryRepository = menuCategoryRepository;
            _restaurantRepository = restaurantRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MenuCategory> CreateCategoryAsync(CreateMenuCategoryDto request, string userId)
        {
            // Get the logged-in user's ID from the JWT token (through HttpContext)
            //var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("Service " + userId);
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            // Retrieve the restaurant that belongs to the current user (logged in user)
            var restaurant = await _restaurantRepository.GetByUserIdAsync(userId);
            if (restaurant == null)
            {
                throw new UnauthorizedAccessException("You do not have access to this restaurant.");
            }


            // Ensure that the restaurant has a menu (it should, as created at registration)
            /*            if (restaurant.Menu == null)
                        {
                            throw new InvalidOperationException("This restaurant does not have a menu. Please create a menu first.");
                        }*/

            // Create the MenuCategory object based on the request
            var menuCategory = new MenuCategory
            {
                Name = request.Name,
                MenuId = restaurant.Menu.Id, // This ensures the category is added to the correct menu
                Menu = restaurant.Menu
            };



            // Save the category to the repository
            await _menuCategoryRepository.AddAsync(menuCategory);
            await _menuCategoryRepository.SaveChangesAsync();

            return menuCategory;
        }

    }
}
