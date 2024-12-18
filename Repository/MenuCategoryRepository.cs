﻿using Microsoft.EntityFrameworkCore;
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

/*        public async Task<IEnumerable<MenuCategory>> GetMenuCategoriesByMenuIdAsync(int menuId)
        {
            return await _context.MenuCategories
                .Where(mc => mc.MenuId == menuId)
                .ToListAsync();
        }*/
        public async Task<List<MenuCategory>> GetMenuCategoriesWithSubAndProductsAsync(int menuId)
        {
            return await _context.Menus
                .Where(m => m.Id == menuId)
                .SelectMany(m => m.MenuCategories)
                .Include(c => c.MenuSubCategories)
                .ThenInclude(sc => sc.Products) // Include products directly in the category
                .Include(c => c.Products)
                .ToListAsync();
        }
        public async Task<MenuCategory> GetCategoryAsync(int categoryId)
        {
            return await _context.MenuCategories
                .Include(mc => mc.MenuSubCategories) 
                .Include(mc => mc.Products)          
                .FirstOrDefaultAsync(mc => mc.Id == categoryId);
        }

        public async Task<MenuCategory> GetCategoryByIdForRestaurantAsync(int restaurantId, int categoryId)
        {
            return await _context.MenuCategories
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.Menu.RestaurantId == restaurantId);
        }

        public void Remove(MenuCategory category)
        {
            _context.MenuCategories.Remove(category);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
