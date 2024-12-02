using SmartMenuManagerApp.Dto;
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
        private readonly IRestaurantRepository _restaurantRepository;

        public ProductService(IProductRepository productRepository, IMenuCategoryRepository menuCategoryRepository, IMenuSubCategoryRepository menuSubCategoryRepository, IRestaurantRepository restaurantRepository)
        {
            _productRepository = productRepository;
            _menuCategoryRepository = menuCategoryRepository;
            _menuSubCategoryRepository = menuSubCategoryRepository;
            _restaurantRepository = restaurantRepository;
        }

        public async Task<Product> AddProductAsync(CreateProductDto request, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            // Retrieve the restaurant that belongs to the current user (logged in user)
            var restaurant = await _restaurantRepository.GetByUserIdAsync(userId);
            if (restaurant == null)
            {
                throw new UnauthorizedAccessException("You do not have access to this restaurant.");
            }

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

            // Add product options if provided
            foreach (var optionDto in request.ProductOptions)
            {
                var productOption = new ProductOption
                {
                    Name = optionDto.Name,
                };

                // Add option details if provided
                foreach (var detailDto in optionDto.OptionDetails)
                {
                    productOption.OptionDetails.Add(new OptionDetail
                    {
                        Name = detailDto.Name,
                        AdditionalPrice = detailDto.AdditionalPrice
                    });
                }

                product.ProductOptions.Add(productOption);
            }


            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            if (request.LabelIds != null && request.LabelIds.Any())
            {
                var labels = await _productRepository.GetLabelsByIdsAsync(request.LabelIds);

                if (!labels.Any())
                {
                    throw new Exception("Invalid label IDs provided.");
                }

                var productLabels = labels.Select(label => new ProductLabel
                {
                    ProductId = product.Id,
                    LabelId = label.Id
                });

                await _productRepository.AddProductLabelsAsync(productLabels);
            }

            

            return product;
        }

        public async Task<DeleteResultDto> DeleteProductAsync(int productId, int restaurantId)
        {
            var product = await _productRepository.GetProductByIdForRestaurantAsync(restaurantId, productId);

            if (product == null)
            {
                return new DeleteResultDto
                {
                    Status = "error",
                    Message = "Product not found or does not belong to the restaurant."
                };
            }

            try
            {

                _productRepository.Remove(product);
                await _menuCategoryRepository.SaveChangesAsync(); // Commit changes to database

                // Return success result
                return new DeleteResultDto
                {
                    Status = "success",
                    Message = "Product deleted successfully",
                    Data = new { Id = productId }
                };
            }
            catch (Exception ex)
            {
                // Return error if something goes wrong
                return new DeleteResultDto
                {
                    Status = "error",
                    Message = "An error occurred while deleting the product",
                    Error = ex.Message
                };
            }
        }
    }
}
