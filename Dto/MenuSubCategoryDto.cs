namespace SmartMenuManagerApp.Dto
{
    public class MenuSubCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
