using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SmartMenuManagerApp.Models
{
    public class Restaurant
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Restaurant name
        public string Address { get; set; } // Restaurant address
        // Opening and closing times
        public TimeSpan OpeningTime { get; set; } // Store opening time
        public TimeSpan ClosingTime { get; set; } // Store closing time
        public string PosProvider { get; set; }


        // Foreign Key - linked to the user who owns this restaurant
        public string UserId { get; set; }
        public User User { get; set; } // Navigation property to ApplicationUser

        // Navigation properties
        public ICollection<MyTable> MyTables { get; set; } = new List<MyTable>(); // Tables in the restaurant
        public Menu Menu { get; set; } 
        public Theme Theme { get; set; } // Customizable theme for the restaurant's menu
    }
}
