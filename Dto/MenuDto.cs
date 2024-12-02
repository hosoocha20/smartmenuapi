namespace SmartMenuManagerApp.Dto
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MenuCategoryDto> Categories { get; set; } = new List<MenuCategoryDto>();
    }
}
