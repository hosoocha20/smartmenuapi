using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IMenuSubCategoryRepository
    {
        Task AddAsync(MenuSubCategory menuSubCategory);
        void Remove(MenuSubCategory menuSubCategory);
        Task<MenuCategory> GetMenuCategoryWithSubCategoriesAsync(int menuCategoryId);
        Task<MenuSubCategory> GetSubCategoryAsync(int subCategoryId);
    }
}
