using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;
using SmartMenuManagerApp.Repository;
using System.Security.Claims;

namespace SmartMenuManagerApp.Services
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IMenuSubCategoryRepository _menuSubCategoryRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        public MenuCategoryService(IMenuCategoryRepository menuCategoryRepository,
                                    IMenuSubCategoryRepository menuSubCategoryRepository,
                                    IMenuRepository menuRepository,
                                    IRestaurantRepository restaurantRepository)
        {
            _menuCategoryRepository = menuCategoryRepository;
            _menuSubCategoryRepository = menuSubCategoryRepository;
            _menuRepository = menuRepository;
            _restaurantRepository = restaurantRepository;
  
        }

        //Creating Menu Category Service
        public async Task<MenuCategory> CreateCategoryAsync(CreateMenuCategoryDto request, string userId)
        {
            // Get the logged-in user's ID from the JWT token (through HttpContext)
            //var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //Console.WriteLine("Service " + userId);
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

        //Creating Sub Menu Category Service
        public async Task<MenuSubCategory> CreateSubCategoryAsync(CreateMenuSubCategoryDto request, string userId)
        {
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

            // Check if the parent category exists
            var category = await _menuSubCategoryRepository.GetMenuCategoryWithSubCategoriesAsync(request.MenuCategoryId);

            if (category == null)
                throw new InvalidOperationException("MenuCategory not found.");

            var subCategory = new MenuSubCategory
            {
                Name = request.Name,
                MenuCategoryId = request.MenuCategoryId
            };
            await _menuSubCategoryRepository.AddAsync(subCategory);
            await _menuCategoryRepository.SaveChangesAsync();

            return subCategory;

        }

        public async Task<IEnumerable<MenuCategory>> GetUserMenuCategoriesAsync(string userId)
        {
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

            // Fetch the categories
            return await _menuCategoryRepository.GetMenuCategoriesByMenuIdAsync(restaurant.Menu.Id);
        }

        public async Task<UserMenuDto> GetUserMenuAsync(string userId)
        {
            var restaurant = await _restaurantRepository.GetByUserIdAsync(userId);

            if (restaurant == null)
                throw new UnauthorizedAccessException("You do not have access to this restaurant.");

            var menu = await _menuRepository.GetMenuWithDetailsAsync(restaurant.Id);

            if (menu == null)
                return null;

            return new UserMenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Categories = menu.MenuCategories.Select(c => new MenuCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    SubCategories = c.MenuSubCategories.Select(sc => new MenuSubCategoryDto
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        Products = sc.Products.Select(p => new ProductDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            ImgUrl = p.ImgUrl,
                            Labels = p.ProductLabels.Select(pl => new LabelDto
                            {
                                Id = pl.Label.Id,
                                Name = pl.Label.Name,
                            }).ToList()
                        }).ToList()
                    }).ToList(),
                    Products = c.Products.Where(p => !c.MenuSubCategories.Any(sc => sc.Products.Contains(p))) // Exclude products that are already in a subcategory
                                         .Select(p => new ProductDto
                                         {
                                             Id = p.Id,
                                             Name = p.Name,
                                             Description = p.Description,
                                             Price = p.Price,
                                             ImgUrl = p.ImgUrl,
                                             Labels = p.ProductLabels.Select(pl => new LabelDto
                                             {
                                                 Id = pl.Label.Id,
                                                 Name = pl.Label.Name,
                                             }).ToList()
                                         }).ToList()
                }).ToList()
            };
        }
    }
}
