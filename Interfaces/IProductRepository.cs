using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<IEnumerable<Label>> GetLabelsByIdsAsync(List<int> labelIds);
        Task AddProductLabelsAsync(IEnumerable<ProductLabel> productLabels);
        Task SaveChangesAsync();
    }
}
