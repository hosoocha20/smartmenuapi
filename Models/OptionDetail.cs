namespace SmartMenuManagerApp.Models
{
    public class OptionDetail
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Name of the option detail
        public decimal AdditionalPrice { get; set; } // Additional price for this option detail

        // Foreign Key
        public int ProductOptionId { get; set; }
        public ProductOption ProductOption { get; set; } // Navigation property to Option
    }
}
