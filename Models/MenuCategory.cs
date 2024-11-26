namespace SmartMenuManagerApp.Models
{
    public class MenuCategory
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Category name

        // Foreign Key - each category belongs to a menu
        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; } // Navigation property to Menu

        // Navigation properties
        public virtual ICollection<MenuSubCategory> MenuSubCategories { get; set; } = new List<MenuSubCategory>(); // Subcategories
        public virtual ICollection<Product> Products { get; set; } = new List<Product>(); 
    }
}
