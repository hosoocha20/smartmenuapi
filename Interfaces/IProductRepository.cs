using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task SaveChangesAsync();
    }
}
