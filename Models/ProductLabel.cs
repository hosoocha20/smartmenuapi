namespace SmartMenuManagerApp.Models
{
    public class ProductLabel
    {
        public int ProductId { get; set; } // Foreign key to Product
        public Product Product { get; set; } // Navigation property

        public int LabelId { get; set; } // Foreign key to Label
        public Label Label { get; set; } // Navigation property
    }
}
