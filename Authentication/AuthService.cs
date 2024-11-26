using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartMenuManagerApp.Configurations;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Dto;

using SmartMenuManagerApp.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartMenuManagerApp.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly DataContext _context;

        public AuthService(UserManager<User> userManager, IJwtService jwtService, DataContext context)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _context = context;
        }

        // RegisterAsync - Handles User Registration
        public async Task<ApiResponse> RegisterAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new ApiResponse(false, "Email is already in use");
            }

            // Create the new user

 

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.Email, // Using email as username
                RestaurantName = registerDto.RestaurantName,
                PhoneNumber = registerDto.PhoneNumber,
                Restaurant = new Restaurant 
                {
                    Name = registerDto.RestaurantName,
                    OpeningTime = registerDto.OpeningTime,
                    ClosingTime = registerDto.ClosingTime,
                    Address = registerDto.Address,
                    PosProvider = registerDto.PosProvider,
                    Menu = new Menu  // Create and associate the default menu
                    {
                        Name = registerDto.RestaurantName + " Menu"
                    }
                }
            };

            // Create user
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            await _context.SaveChangesAsync();
            if (!result.Succeeded)
            {
                return new ApiResponse(false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Create a restaurant for the user
 /*           var restaurant = new Restaurant
            {
                Name = registerDto.RestaurantName,
                OpeningTime = registerDto.OpeningTime,
                ClosingTime = registerDto.ClosingTime,
                Address = registerDto.Address,
                PosProvider = registerDto.PosProvider,
                UserId = user.Id, // Associate restaurant with the user
                Menu = new Menu  // Create and associate the default menu
                    {
                        Name = registerDto.RestaurantName + " Menu",
                        // Optionally, set more properties for the menu here
                    }
            };

            // Save the restaurant to the database
            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();  // Save user and restaurant to the database*/

            // Create default menu for the restaurant
/*            var menu = new Menu
            {
                Name = registerDto.RestaurantName + " Menu",
                RestaurantId = restaurant.Id
            };

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();*/

            // Generate JWT Token for the user
            var jwtToken = _jwtService.GenerateJwtToken(user);

            return new ApiResponse(true, "User and restaurant and default menu created successfully", jwtToken);
        }

        // LoginAsync - Handles User Login
        public async Task<ApiResponse> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new ApiResponse(false, "Invalid credentials");
            }

            // Generate JWT Token
            var jwtToken = _jwtService.GenerateJwtToken(user); // Generate JWT token
            return new ApiResponse(true, "Login successful", jwtToken);
        }



    }
}
