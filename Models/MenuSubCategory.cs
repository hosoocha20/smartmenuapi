namespace SmartMenuManagerApp.Models
{
    public class MenuSubCategory
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Subcategory name

        // Foreign Key - linked to a category
        public int MenuCategoryId { get; set; }
        public MenuCategory MenuCategory { get; set; } // Navigation property to Category
                                                       
        public ICollection<Product> Products { get; set; } // Navigation property for products within this subcategory
    }
}
