using SmartMenuManagerApp.Dto;

namespace SmartMenuManagerApp.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantDto> GetRestaurantAsync(string userId);
    }
}
