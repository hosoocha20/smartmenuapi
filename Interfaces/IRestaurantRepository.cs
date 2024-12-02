using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Interfaces
{
    public interface IRestaurantRepository
    {
        // Method to get the restaurant based on the logged-in user ID
        Task<Restaurant> GetByUserIdAsync(string userId);

        //Method to fetch restaurants by ID
        Task<Restaurant> GetByIdAsync(int id);

        //Get Restaurant Details
        Task<Restaurant> GetRestaurantWithDetailsAsync(int restaurantId);
    }
}
