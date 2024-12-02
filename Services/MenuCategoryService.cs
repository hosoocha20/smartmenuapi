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
        private readonly IProductRepository _productRepository;
        public MenuCategoryService(IMenuCategoryRepository menuCategoryRepository,
                                    IMenuSubCategoryRepository menuSubCategoryRepository,
                                    IMenuRepository menuRepository,
                                    IRestaurantRepository restaurantRepository,
                                    IProductRepository productRespository)
        {
            _menuCategoryRepository = menuCategoryRepository;
            _menuSubCategoryRepository = menuSubCategoryRepository;
            _menuRepository = menuRepository;
            _restaurantRepository = restaurantRepository;
            _productRepository = productRespository;
  
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

        //Delete Category
        public async Task<DeleteResultDto> DeleteCategoryAsync(int categoryId, int restaurantId)
        {
            var category = await _menuCategoryRepository.GetCategoryByIdForRestaurantAsync(restaurantId, categoryId);

            if (category == null)
            {
                return new DeleteResultDto
                {
                    Status = "error",
                    Message = "Category not found or does not belong to the restaurant."
                };
            }

            try
            {
                
                _menuCategoryRepository.Remove(category);
                await _menuCategoryRepository.SaveChangesAsync(); // Commit changes to database

                // Return success result
                return new DeleteResultDto
                {
                    Status = "success",
                    Message = "Category deleted successfully",
                    Data = new { Id = categoryId }
                };
            }
            catch (Exception ex)
            {
                // Return error if something goes wrong
                return new DeleteResultDto
                {
                    Status = "error",
                    Message = "An error occurred while deleting the category",
                    Error = ex.Message
                };
            }
        }
    
        //Get User Categories for Table
        public async Task<List<UserTableMenuCategoryDto>> GetAllTableMenuCategoriesAsync(string userId)
        {
            var restaurant = await _restaurantRepository.GetByUserIdAsync(userId);

            if (restaurant == null)
                throw new UnauthorizedAccessException("You do not have access to this restaurant.");

            var menuCategories = await _menuCategoryRepository.GetMenuCategoriesWithSubAndProductsAsync(restaurant.Menu.Id);

            if (menuCategories == null)
                return null;

            return menuCategories.Select(c => new UserTableMenuCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                SubCategoriesNo = c.MenuSubCategories.Count(), // Count of subcategories
                ProductsNo = c.Products.Count() + c.MenuSubCategories.SelectMany(sc => sc.Products).Count() // Count of products in category and subcategories
            }).ToList();
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

        //Delete SubCategory
        public async Task<DeleteResultDto> DeleteSubCategoryAsync(int subCategoryId, int restaurantId)
        {
            var subCategory = await _menuSubCategoryRepository.GetSubCategoryByIdForRestaurantAsync(restaurantId, subCategoryId);
                

            if (subCategory == null)
            {
                return new DeleteResultDto
                {
                    Status = "error",
                    Message = "Sub category not found or does not belong to the restaurant."
                };
            }

            try
            {
              
                if (subCategory.Products != null && subCategory.Products.Any())
                {
                    Console.WriteLine("productsexist");
                    foreach (var product in subCategory.Products)
                    {
                        product.MenuSubCategoryId = null;  // Disassociate from subcategory
                    }

                    _productRepository.RemoveRange(subCategory.Products);
                    await _menuCategoryRepository.SaveChangesAsync();
                }

                _menuSubCategoryRepository.Remove(subCategory);
                await _menuCategoryRepository.SaveChangesAsync(); // Commit changes to database

                // Return success result
                return new DeleteResultDto
                {
                    Status = "success",
                    Message = "Sub category deleted successfully",
                    Data = new { Id = subCategoryId }
                };
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                // Return error if something goes wrong
                return new DeleteResultDto
                {
                    Status = "error",
                    Message = "An error occurred while deleting the sub category",
                    Error = innerException
                };
            }
        }
        //Get All Subcategories
        public async Task<List<UserMenuSubCategoryDto>> GetAllSubCategoryAsync(string userId)
        {
            var restaurant = await _restaurantRepository.GetByUserIdAsync(userId);

            if (restaurant == null)
                throw new UnauthorizedAccessException("You do not have access to this restaurant.");

            var menu = await _menuRepository.GetMenuWithDetailsAsync(restaurant.Id);

            if (menu == null)
                return null;
            var subcategories = menu.MenuCategories
                .SelectMany(c => c.MenuSubCategories) // Flatten all subcategories from all categories
                .Select(sc => new UserMenuSubCategoryDto
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    ParentCategory = new ParentCategoryDto // Mapping the parent category
                    {
                        Id = sc.MenuCategory.Id,
                        Name = sc.MenuCategory.Name
                    },
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
                            Name = pl.Label.Name
                        }).ToList(),
                        Options = p.ProductOptions.Select(po => new ProductOptionDto
                         {
                             Id = po.Id,
                             Name = po.Name,
                             OptionDetails = po.OptionDetails.Select(od => new OptionDetailDto
                             {
                                  Id = od.Id,
                                  Name = od.Name,
                                  AdditionalPrice = od.AdditionalPrice
                              }).ToList()
                         }).ToList()
                    }).ToList()
                })
                .ToList();
            return subcategories;
        }

/*        public async Task<IEnumerable<MenuCategory>> GetUserMenuCategoriesAsync(string userId)
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
        }*/

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
                            }).ToList(),
                            Options = p.ProductOptions.Select(po => new ProductOptionDto
                            {
                                Id = po.Id,
                                Name = po.Name,
                                OptionDetails = po.OptionDetails.Select(od => new OptionDetailDto
                                {
                                    Id = od.Id,
                                    Name = od.Name,
                                    AdditionalPrice = od.AdditionalPrice
                                }).ToList()
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
                                             }).ToList(),
                                             Options = p.ProductOptions.Select(po => new ProductOptionDto
                                             {
                                                 Id = po.Id,
                                                 Name = po.Name,
                                                 OptionDetails = po.OptionDetails.Select(od => new OptionDetailDto
                                                 {
                                                     Id = od.Id,
                                                     Name = od.Name,
                                                     AdditionalPrice = od.AdditionalPrice
                                                 }).ToList()
                                             }).ToList()
                                         }).ToList()
                }).ToList()
            };
        }
    }
}
