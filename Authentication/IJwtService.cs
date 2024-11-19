using SmartMenuManagerApp.Models;
using System.Security.Claims;

namespace SmartMenuManagerApp.Authentication
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user); // Method to generate JWT token for a user
        ClaimsPrincipal ValidateToken(string token); // Method to validate an incoming token
    }
}
