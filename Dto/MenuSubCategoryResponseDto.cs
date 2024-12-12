namespace SmartMenuManagerApp.Dto
{
    public class MenuSubCategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MenuCategoryId { get; set; }
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
