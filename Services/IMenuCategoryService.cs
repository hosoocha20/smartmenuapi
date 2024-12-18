﻿using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Services
{
    public interface IMenuCategoryService
    {
        Task<MenuCategory> CreateCategoryAsync(CreateMenuCategoryDto request, string userId);
        Task<DeleteResultDto> DeleteCategoryAsync(int categoryId, int restaurantId);
        Task<MenuSubCategory> CreateSubCategoryAsync(CreateMenuSubCategoryDto request, string userId);
        Task<DeleteResultDto> DeleteSubCategoryAsync(int subCategoryId, int restaurantId);

/*        Task<IEnumerable<MenuCategory>> GetUserMenuCategoriesAsync(string userId);*/
        Task<UserMenuDto> GetUserMenuAsync(string userId);
        Task<List<UserMenuSubCategoryDto>> GetAllSubCategoryAsync(string userId);
        Task<List<UserTableMenuCategoryDto>> GetAllTableMenuCategoriesAsync(string userId);
    }
}
