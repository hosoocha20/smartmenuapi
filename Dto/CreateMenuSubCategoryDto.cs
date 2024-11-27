using System.ComponentModel.DataAnnotations;

namespace SmartMenuManagerApp.Dto
{
    public class CreateMenuSubCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public int MenuCategoryId { get; set; }
    }
}
