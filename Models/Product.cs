namespace SmartMenuManagerApp.Models
{
    public class Product
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Product name
        public string Description { get; set; } // Product description
        public decimal Price { get; set; } // Product price
        public string ImgUrl { get; set; } //Get Image URL from cloud storage

        // Foreign Keys
        public int MenuCategoryId { get; set; }
        public int? MenuSubCategoryId { get; set; } // Nullable if product isn't in a subcategory
        public MenuCategory MenuCategory { get; set; } // Navigation property to Category
        public MenuSubCategory? MenuSubCategory { get; set; } // Navigation property to SubCategory

        // Navigation property
        public ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>(); // Options for the product
        // Collection for Labels
        public ICollection<ProductLabel> ProductLabels { get; set; }
    }
}
