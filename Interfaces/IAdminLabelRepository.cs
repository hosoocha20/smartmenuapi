using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IAdminLabelRepository
    {
        Task AddAsync(Label label);
        Task SaveChangesAsync();
    }
}
