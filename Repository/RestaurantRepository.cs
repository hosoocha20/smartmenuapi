using Microsoft.EntityFrameworkCore;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Repository
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly DataContext _context;
        public RestaurantRepository(DataContext context)
        {
            _context = context;
        }
        // Get restaurant by user ID (user owns the restaurant)
        public async Task<Restaurant> GetByUserIdAsync(string userId)
        {
            return await _context.Restaurants
                //.AsNoTracking()
                .Include(r => r.Menu)  // Include the Menu in the result since we need to check if the restaurant has one
                .FirstOrDefaultAsync(r => r.UserId == userId);
        }

        //Get by restaurant ID 
        public async Task<Restaurant> GetByIdAsync(int id)
        {
            return await _context.Restaurants
                .Include(r => r.Menu) // Include menu to check
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        //Get Restaurant Details
        public async Task<Restaurant> GetRestaurantWithDetailsAsync(int restaurantId)
        {
            return await _context.Restaurants
        .Include(r => r.MyTables)
        .Include(r => r.Menu)
            .ThenInclude(m => m.MenuCategories)
            .ThenInclude(c => c.MenuSubCategories)
                .ThenInclude(sc => sc.Products)
                    .ThenInclude(p => p.ProductLabels)
                        .ThenInclude(pl => pl.Label)
        .Include(r => r.Menu)
        .ThenInclude(m => m.MenuCategories)
            .ThenInclude(c => c.Products)
                .ThenInclude(p => p.ProductLabels)
                    .ThenInclude(pl => pl.Label)
        .Include(r => r.Menu)
        .ThenInclude(m => m.MenuCategories)
            .ThenInclude(c => c.MenuSubCategories)
                .ThenInclude(sc => sc.Products)
                    .ThenInclude(p => p.ProductOptions) // Include ProductOptions
                        .ThenInclude(po => po.OptionDetails) // Include OptionDetails for each option
        .Include(r => r.Menu)
        .ThenInclude(m => m.MenuCategories)
            .ThenInclude(c => c.Products)
                .ThenInclude(p => p.ProductOptions)
                    .ThenInclude(po => po.OptionDetails)

        .Include(r => r.Theme)
        .FirstOrDefaultAsync(r => r.Id == restaurantId);
        }
    }
}
