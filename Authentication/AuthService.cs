﻿using Microsoft.AspNetCore.Identity;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly DataContext _context;

        public AuthService(UserManager<User> userManager, IJwtService jwtService, RoleManager<IdentityRole> roleManager, DataContext context)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
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
                    },
                    Theme = new Theme  // Create and associate a default theme
                    {
                        HeaderColor = "#FFFFFF",
                        HeaderTextColor = "#000000",
                        SubHeaderImgUrl = null,  // No default image
                        SidebarColor = "#F4F4F4",
                        SidebarTextColor = "#333333",
                        SidebarSelectedColor = "#FF5733",
                        BodyColor = "#FAFAFA",
                        BodyHeaderTextColor = "#111111",
                        MenuItemBodyColor = "#FFFFFF",
                        MenuItemTextColor = "#444444",
                        MenuItemPriceColor = "#FF0000",
                        ButtonColor = "#007BFF",
                        ButtonTextColor = "#FFFFFF",
                        SubCategoryTextColor = "#222222",
                        LogoUrl = "default-logo.png" // Placeholder logo
                    },

                    MyTables = new List<MyTable>() // Initialize an empty list of tables
                }
            };

            // Create user
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description))); // Return errors for better debugging
            }

            // Check if the role exists, and create it if not
            if (!await _roleManager.RoleExistsAsync(registerDto.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(registerDto.Role));
            }

            // Assign the user to the role
            await _userManager.AddToRoleAsync(user, registerDto.Role);
            await _context.SaveChangesAsync();


            // Generate JWT Token for the user
            var jwtToken = _jwtService.GenerateJwtToken(user);

            return new ApiResponse(true, "User and restaurant and default menu created successfully", jwtToken);
        }

        // LoginAsync - Handles User Login
        public async Task<ApiResponse> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.Users
            .Include(u => u.Restaurant) // Include the Restaurant to ensure it's loaded
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new ApiResponse(false, "Invalid credentials");
            }

            // Check if Restaurant is associated with the User
            if (user.Restaurant == null)
            {
                return new ApiResponse(false, "Restaurant ID is not associated with the user.");
            }

            // Generate JWT Token
            var jwtToken = _jwtService.GenerateJwtToken(user); // Generate JWT token
            return new ApiResponse(true, "Login successful", jwtToken);
        }



    }
}
