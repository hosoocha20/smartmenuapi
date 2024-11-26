using Microsoft.EntityFrameworkCore;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;
using System;

namespace SmartMenuManagerApp.Repository
{
    public class MenuCategoryRepository : IMenuCategoryRepository
    {
        private readonly DataContext _context;

        public MenuCategoryRepository(DataContext context)
        {
            _context = context;
        }
        public async Task AddAsync(MenuCategory menuCategory)
        {
            await _context.MenuCategories.AddAsync(menuCategory);
        }

        public async Task<Menu> GetMenuByRestaurantIdAsync(int restaurantId)
        {
            return await _context.Menus
                .Include(m => m.MenuCategories)
                .FirstOrDefaultAsync(m => m.RestaurantId == restaurantId);
        }

        public async Task<IEnumerable<MenuCategory>> GetMenuCategoriesByMenuIdAsync(int menuId)
        {
            return await _context.MenuCategories
                .Where(mc => mc.MenuId == menuId)
                .ToListAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
