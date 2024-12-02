using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IMenuCategoryRepository
    {
        Task<Menu> GetMenuByRestaurantIdAsync(int restaurantId);
        /*Task<IEnumerable<MenuCategory>> GetMenuCategoriesByMenuIdAsync(int menuId);*/
        Task<MenuCategory> GetCategoryAsync(int categoryId);
        Task<MenuCategory> GetCategoryByIdForRestaurantAsync(int restaurantId, int categoryId);
        Task<List<MenuCategory>> GetMenuCategoriesWithSubAndProductsAsync(int menuId);

        void Remove(MenuCategory category);
        Task AddAsync(MenuCategory menuCategory);
        Task SaveChangesAsync();
    }
}
