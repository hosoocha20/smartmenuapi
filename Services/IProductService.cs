using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Services
{
    public interface IProductService
    {
        Task<Product> AddProductAsync(CreateProductDto request, string userId);
    }
}
