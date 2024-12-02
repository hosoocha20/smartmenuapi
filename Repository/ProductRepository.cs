using Microsoft.EntityFrameworkCore;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _context;

        public ProductRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task<IEnumerable<Label>> GetLabelsByIdsAsync(List<int> labelIds)
        {
            return await _context.Labels.Where(label => labelIds.Contains(label.Id)).ToListAsync();
        }

        public async Task AddProductLabelsAsync(IEnumerable<ProductLabel> productLabels)
        {
            await _context.ProductLabels.AddRangeAsync(productLabels);
            await _context.SaveChangesAsync();
        }

        public async Task<Product> GetProductByIdForRestaurantAsync(int restaurantId, int productId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.MenuCategory.Menu.RestaurantId == restaurantId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void Remove(Product product)
        {
            _context.Products.Remove(product);
        }

        public async Task RemoveProductsBySubCategoryIdAsync(int subCategoryId)
        {
            var products = await _context.Products
                .Where(p => p.MenuSubCategoryId == subCategoryId)
                .ToListAsync();

            if (products.Any())
            {
                _context.Products.RemoveRange(products);
            }
        }

        public void RemoveRange(IEnumerable<Product> products)
        {
            _context.Products.RemoveRange(products);
            
        }


    }
}
