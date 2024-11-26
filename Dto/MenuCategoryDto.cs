using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Dto
{
    public class MenuCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MenuSubCategoryDto> SubCategories { get; set; } = new List<MenuSubCategoryDto>();
        public List<ProductDto> Products { get; set; } = new List<ProductDto>(); // Products not in subcategories
    }
}
