using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IAdminLabelRepository
    {
        Task AddAsync(Label label);
        Task DeleteAsync(Label label);
        Task SaveChangesAsync();

        Task<Label> GetByIdAsync(int id);
        
    }
}
