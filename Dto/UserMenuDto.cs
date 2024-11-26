using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Dto
{
    public class UserMenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MenuCategoryDto> Categories { get; set; } = new List<MenuCategoryDto>();
    }
}
