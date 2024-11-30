namespace SmartMenuManagerApp.Dto
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
        public int MenuCategoryId { get; set; }
        public int? MenuSubCategoryId { get; set; }
        public List<int> LabelIds { get; set; } = new List<int>(); //For optional Label
        public List<CreateProductOptionDto> ProductOptions { get; set; } = new List<CreateProductOptionDto>();
    }
}
