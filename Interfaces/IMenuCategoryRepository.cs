using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IMenuCategoryRepository
    {
        Task<Menu> GetMenuByRestaurantIdAsync(int restaurantId);
        Task AddAsync(MenuCategory menuCategory);
        Task SaveChangesAsync();
    }
}
