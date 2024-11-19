using Microsoft.AspNetCore.Identity;

namespace SmartMenuManagerApp.Models
{
    public class User: IdentityUser
    {
        // Basic user properties inherited from IdentityUser:
        // Id - unique identifier
        // UserName - unique username
        // Email - user email address
        // PasswordHash - hashed password (handled by Identity)
        // SecurityStamp - used to verify account security

        // Additional properties specific to your application
        public string FullName { get; set; }
        public string RestaurantName { get; set; }
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow; // Date the user registered
                                                                        // Navigation property - one user can own multiple restaurants
        public Restaurant Restaurant { get; set; }

    }
}
