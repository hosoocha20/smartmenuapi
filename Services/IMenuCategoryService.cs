﻿using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Services
{
    public interface IMenuCategoryService
    {
        Task<MenuCategory> CreateCategoryAsync(CreateMenuCategoryDto request, string userId);
    }
}