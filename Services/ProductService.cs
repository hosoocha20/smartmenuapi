﻿using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;
using SmartMenuManagerApp.Repository;

namespace SmartMenuManagerApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IMenuSubCategoryRepository _menuSubCategoryRepository;

        public ProductService(IProductRepository productRepository, IMenuCategoryRepository menuCategoryRepository, IMenuSubCategoryRepository menuSubCategoryRepository)
        {
            _productRepository = productRepository;
            _menuCategoryRepository = menuCategoryRepository;
            _menuSubCategoryRepository = menuSubCategoryRepository;
        }

        public async Task<Product> AddProductAsync(CreateProductDto request)
        {
            // Retrieve the category and subcategory (if provided)
            var category = await _menuCategoryRepository.GetCategoryAsync(request.MenuCategoryId);
            if (category == null)
            {
                throw new InvalidOperationException("Menu category not found.");
            }
            // Check for if the category has subcategories -  if yes then product must be assigned to a subcategory
            if (category.MenuSubCategories.Any())
            {
                if (request.MenuSubCategoryId == null)
                {
                    throw new InvalidOperationException("This category has subcategories. Product must be assigned to a subcategory.");
                }

                // Check if the provided subcategory is valid for the category
                var validSubCategory = category.MenuSubCategories
                                               .Any(sc => sc.Id == request.MenuSubCategoryId);

                if (!validSubCategory)
                {
                    throw new InvalidOperationException("Invalid subcategory for this category.");
                }
            }


            // Create Product Entity
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                ImgUrl = request.ImgUrl,
                MenuCategoryId = request.MenuCategoryId,
                MenuSubCategoryId = request.MenuSubCategoryId
            };

            // Save the product
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return product;
        }
    }
}
