using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Services
{
    public interface IMenuCategoryService
    {
        Task<MenuCategory> CreateCategoryAsync(CreateMenuCategoryDto request, string userId);
        Task<MenuSubCategory> CreateSubCategoryAsync(CreateMenuCategoryDto request, string userId, int menuCategoryId);
        Task<IEnumerable<MenuCategory>> GetUserMenuCategoriesAsync(string userId);
        Task<UserMenuDto> GetUserMenuAsync(string userId);
    }
}
