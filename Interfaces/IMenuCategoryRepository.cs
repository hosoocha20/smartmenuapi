﻿using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IMenuCategoryRepository
    {
        Task<Menu> GetMenuByRestaurantIdAsync(int restaurantId);
        /*Task<IEnumerable<MenuCategory>> GetMenuCategoriesByMenuIdAsync(int menuId);*/
        Task<MenuCategory> GetCategoryAsync(int categoryId);
        Task<List<MenuCategory>> GetMenuCategoriesWithSubAndProductsAsync(int menuId);

        Task AddAsync(MenuCategory menuCategory);
        Task SaveChangesAsync();
    }
}
