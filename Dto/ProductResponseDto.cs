namespace SmartMenuManagerApp.Dto
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }

        public int MenuCategoryId { get; set; }
        public int? MenuSubCategoryId { get; set; }
        public List<LabelDto> Labels { get; set; } = new List<LabelDto>();
        public List<ProductOptionDto> Options { get; set; } = new List<ProductOptionDto>();
    }
}
