using Microsoft.EntityFrameworkCore;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly DataContext _context;

        public MenuRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Menu> GetMenuWithDetailsAsync(int restaurantId)
        {
            return await _context.Menus
                .Include(m => m.MenuCategories)
                    .ThenInclude(c => c.MenuSubCategories)
                        .ThenInclude(sc => sc.Products) // Include products in subcategories
                .Include(m => m.MenuCategories)
                    .ThenInclude(c => c.Products) // Include products directly in the category
                .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId);
        }
    }
}
