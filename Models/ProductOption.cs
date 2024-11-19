namespace SmartMenuManagerApp.Models
{
    public class ProductOption
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Option name

        // Foreign Key
        public int ProductId { get; set; }
        public Product Product { get; set; } // Navigation property to Product

        // Navigation property
        public ICollection<OptionDetail> OptionDetails { get; set; } = new List<OptionDetail>(); // Details of options
    }
}
