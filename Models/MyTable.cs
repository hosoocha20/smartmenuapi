namespace SmartMenuManagerApp.Models
{
    public class MyTable
    {
        public int Id { get; set; } // Primary Key
        public string Code { get; set; } // Unique code for the table (for QR purposes)

        // Foreign Key - linked to a specific restaurant
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } // Navigation property to Restaurant
    }
}
