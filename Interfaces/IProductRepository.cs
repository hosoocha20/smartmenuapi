using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<IEnumerable<Label>> GetLabelsByIdsAsync(List<int> labelIds);
        Task AddProductLabelsAsync(IEnumerable<ProductLabel> productLabels);
        Task<Product> GetProductByIdForRestaurantAsync(int restaurantId, int productId);
        Task SaveChangesAsync();
        void Remove(Product product);
        Task RemoveProductsBySubCategoryIdAsync(int subCategoryId);
        void RemoveRange(IEnumerable<Product> products);
    }
}
