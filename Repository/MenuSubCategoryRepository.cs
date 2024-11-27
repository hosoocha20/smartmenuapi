using Microsoft.EntityFrameworkCore;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Repository
{
    public class MenuSubCategoryRepository : IMenuSubCategoryRepository
    {
        private readonly DataContext _context;
        public MenuSubCategoryRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MenuSubCategory menuSubCategory)
        {
            await _context.MenuSubCategories.AddAsync(menuSubCategory);
        }

        public async Task<MenuCategory> GetMenuCategoryWithSubCategoriesAsync(int menuCategoryId)
        {
            return await _context.MenuCategories
                .Include(c => c.MenuSubCategories) 
                .FirstOrDefaultAsync(c => c.Id == menuCategoryId);
        }
    }
}
