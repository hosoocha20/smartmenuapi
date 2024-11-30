using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IMenuRepository
    {
        Task<Menu> GetMenuWithDetailsAsync(int restaurantId);

    }
}
