using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartMenuManagerApp.Authentication;
using SmartMenuManagerApp.Configurations;
using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Models;

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add other services like your custom services, repositories, etc.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();



builder.Services.AddScoped<UserManager<User>>();  
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings")); // JWT Settings

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
    });

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
