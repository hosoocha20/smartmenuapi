using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IMenuCategoryRepository
    {
        Task<MenuCategory> GetByIdAsync(int id);
        Task AddAsync(MenuCategory menuCategory);
        Task SaveChangesAsync();
    }
}
