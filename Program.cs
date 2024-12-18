using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartMenuManagerApp.Authentication;
using SmartMenuManagerApp.Configurations;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;
using SmartMenuManagerApp.Repository;
using SmartMenuManagerApp.Services;
using System;
using System.Text;



var builder = WebApplication.CreateBuilder(args);


// Add configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>();

//Console.WriteLine(builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection"));




// Add services to the container.

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
        //Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add other services like your custom services, repositories, etc.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IMenuCategoryService, MenuCategoryService>();
builder.Services.AddScoped<IAdminLabelService, AdminLabelService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IMenuCategoryRepository, MenuCategoryRepository>();
builder.Services.AddScoped<IMenuCategoryService, MenuCategoryService>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuSubCategoryRepository, MenuSubCategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IAdminLabelRepository, AdminLabelRepository>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



builder.Services.AddScoped<UserManager<User>>();  
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings")); // JWT Settings
builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings")); //Admin Settings

// Add DbContext and configure SQL Server with the connection string
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,        // Retry up to 5 times
        maxRetryDelay: TimeSpan.FromSeconds(30), // Max delay of 30 seconds between retries
        errorNumbersToAdd: null) 
    ));



// Add authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
        Console.WriteLine(jwtSettings.Issuer);

        // Add event handlers for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Jwt_Or_Identity", policy =>
    {
        policy.AuthenticationSchemes.Add(IdentityConstants.ApplicationScheme);
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();

    });
    // New policy for admin-only endpoints
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin"); // Require the "Admin" role
        policy.RequireAuthenticatedUser(); // Ensure the user is authenticated
    });
});

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();

// Create Admin role and Admin user on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateAdminUserAsync(services);
}

// Middleware to log request details
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    Console.WriteLine($"Authorization Header: {context.Request.Headers["Authorization"]}");
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseDeveloperExceptionPage(); // This will show detailed errors in development mode

app.MapControllers();

app.Run();


//Admin
async Task CreateAdminUserAsync(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var adminSettings = services.GetRequiredService<IOptions<AdminSettings>>().Value;

    // Create the admin role if it doesn't exist
    var roleExist = await roleManager.RoleExistsAsync("Admin");
    if (!roleExist)
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Check if the admin user already exists
    var adminUser = await userManager.FindByEmailAsync(adminSettings.Email);
    if (adminUser == null)
    {
        // Create the admin user if they don't exist
        var user = new User
        {
            UserName = adminSettings.Email,
            Email = adminSettings.Email,
            PhoneNumber = "0000000000" // Default placeholder number
        };
        var createUserResult = await userManager.CreateAsync(user, adminSettings.Password);  // Set a strong password for the admin user
        if (createUserResult.Succeeded)
        {
            // Assign the Admin role to the new user
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
