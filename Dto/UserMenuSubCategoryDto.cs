namespace SmartMenuManagerApp.Dto
{
    public class UserMenuSubCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ParentCategoryDto ParentCategory { get; set; } // Add Parent Category
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
