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

        public async Task<MenuCategory> GetByIdAsync(int id)
        {
            return await _context.MenuCategories.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
