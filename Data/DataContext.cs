using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Data
{
    public class DataContext : IdentityDbContext

    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Restaurant> Restaurants { get; set; } // Each user has one restaurant
        public DbSet<MyTable> MyTables { get; set; }           // Tables associated with a restaurant
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; } // Categories in the menu
        public DbSet<MenuSubCategory> MenuSubCategories { get; set; } // Subcategories within menu categories
        public DbSet<Product> Products { get; set; }       // Products in categories/subcategories
        public DbSet<ProductOption> ProductOptions { get; set; } // Product options (e.g., sizes)
        public DbSet<OptionDetail> OptionDetails { get; set; }   // Details within an option (e.g., additional prices)
        public DbSet<Theme> Themes { get; set; } // DbSet for Theme model
        public DbSet<Label> Labels { get; set; }           // DbSet for Label entity
        public DbSet<ProductLabel> ProductLabels { get; set; } // DbSet for the join table

        // Override OnModelCreating to configure relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User and Restaurant: One-to-One Relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Restaurant)
                .WithOne(r => r.User)
                .HasForeignKey<Restaurant>(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restaurant and Menu: One-to-One Relationship
            modelBuilder.Entity<Restaurant>()
                .HasOne(r => r.Menu)
                .WithOne(m => m.Restaurant)
                .HasForeignKey<Menu>(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restaurant and Tables: One-to-Many Relationship
            modelBuilder.Entity<Restaurant>()
                .HasMany(r => r.MyTables)
                .WithOne(t => t.Restaurant)
                .HasForeignKey(t => t.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restaurant and Theme: One-to-One Relationship
            modelBuilder.Entity<Restaurant>()
                .HasOne(r => r.Theme)
                .WithOne(t => t.Restaurant)
                .HasForeignKey<Theme>(t => t.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Menu and Menu Categories: One-to-Many Relationship
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.MenuCategories)
                .WithOne(mc => mc.Menu)
                .HasForeignKey(mc => mc.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuCategory and SubMenuCategory: One-to-Many Relationship
            modelBuilder.Entity<MenuCategory>()
                .HasMany(mc => mc.MenuSubCategories)
                .WithOne(sc => sc.MenuCategory)
                .HasForeignKey(sc => sc.MenuCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuCategory and Products: One-to-Many Relationship
            modelBuilder.Entity<MenuCategory>()
                .HasMany(mc => mc.Products)
                .WithOne(p => p.MenuCategory)
                .HasForeignKey(p => p.MenuCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many relationship between MenuSubCategory and Product
            modelBuilder.Entity<MenuSubCategory>()
                .HasMany(sc => sc.Products)  // A MenuSubCategory can have many Products
                .WithOne(p => p.MenuSubCategory)  // Each Product belongs to one MenuSubCategory
                .HasForeignKey(p => p.MenuSubCategoryId)  // Foreign key in Product
                 .OnDelete(DeleteBehavior.Restrict);  // Restrict delete - handle this in my app - i.e. manually make each associated product to null b4 deleteing subcategory

            // Product and ProductOptions: One-to-Many Relationship
            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductOptions)
                .WithOne(po => po.Product)
                .HasForeignKey(po => po.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductOption and OptionDetails: One-to-Many Relationship
            modelBuilder.Entity<ProductOption>()
                .HasMany(po => po.OptionDetails)
                .WithOne(od => od.ProductOption)
                .HasForeignKey(od => od.ProductOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the many-to-many relationship between Product and Label through ProductLabel
            modelBuilder.Entity<ProductLabel>()
                .HasKey(pl => new { pl.ProductId, pl.LabelId }); // Composite primary key

            modelBuilder.Entity<ProductLabel>()
                .HasOne(pl => pl.Product)
                .WithMany(p => p.ProductLabels)
                .HasForeignKey(pl => pl.ProductId);

            modelBuilder.Entity<ProductLabel>()
                .HasOne(pl => pl.Label)
                .WithMany(l => l.ProductLabels)
                .HasForeignKey(pl => pl.LabelId);



            // Additional configurations for required properties and unique constraints (if necessary)
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.DateRegistered)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.RestaurantName)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();  // Ensures the PasswordHash is non-nullable

            modelBuilder.Entity<User>()
                .Property(u => u.SecurityStamp)
                .IsRequired();  // Ensures the SecurityStamp is non-nullable

            modelBuilder.Entity<User>()
                .Property(u => u.ConcurrencyStamp)
                .IsRequired();  // Ensures the ConcurrencyStamp is non-nullable
            modelBuilder.Entity<User>()
                .Property(u => u.PhoneNumber)
                .IsRequired();  // Ensures the ConcurrencyStamp is non-nullable
            modelBuilder.Entity<User>()
                .Property(u => u.NormalizedEmail)
                .IsRequired();  // Ensures the ConcurrencyStamp is non-nullable

            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)
                .IsRequired();

            modelBuilder.Entity<MenuCategory>()
                .Property(mc => mc.Name)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired();

            modelBuilder.Entity<ProductOption>()
                .Property(po => po.Name)
                .IsRequired();

            modelBuilder.Entity<OptionDetail>()
                .Property(od => od.Name)
                .IsRequired();
        }

    }
    }


