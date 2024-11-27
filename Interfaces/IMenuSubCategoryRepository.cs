using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IMenuSubCategoryRepository
    {
        Task AddAsync(MenuSubCategory menuSubCategory);
        Task<MenuCategory> GetMenuCategoryWithSubCategoriesAsync(int menuCategoryId);
    }
}
