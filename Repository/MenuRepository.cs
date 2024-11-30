using Microsoft.EntityFrameworkCore;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Dto;
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
                .ThenInclude(sc => sc.Products)
                    .ThenInclude(p => p.ProductLabels)
                        .ThenInclude(pl => pl.Label)
        .Include(m => m.MenuCategories)
            .ThenInclude(c => c.Products)
                .ThenInclude(p => p.ProductLabels)
                    .ThenInclude(pl => pl.Label)
        .Include(m => m.MenuCategories)
            .ThenInclude(c => c.MenuSubCategories)
                .ThenInclude(sc => sc.Products)
                    .ThenInclude(p => p.ProductOptions) // Include ProductOptions
                        .ThenInclude(po => po.OptionDetails) // Include OptionDetails for each option
        .Include(m => m.MenuCategories)
            .ThenInclude(c => c.Products)
                .ThenInclude(p => p.ProductOptions)
                    .ThenInclude(po => po.OptionDetails)
        .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId);
        }


    }
}
