using System.Text.Json.Serialization;

namespace SmartMenuManagerApp.Models
{
    public class Menu
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Menu name

        // Foreign Key - each menu belongs to a restaurant
        public int RestaurantId { get; set; }

        
        public virtual Restaurant Restaurant { get; set; } // Navigation property to Restaurant

        // Navigation property
        public virtual ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>(); // Categories in the menu
    }
}
