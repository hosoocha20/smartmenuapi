namespace SmartMenuManagerApp.Models
{
    public class Label
    {
        public int Id { get; set; } // Primary key
        public string Name { get; set; } // Label name (e.g., Vegan, Spicy)
 

        public ICollection<ProductLabel> ProductLabels { get; set; } // Navigation property for many-to-many relationship
    }
}
