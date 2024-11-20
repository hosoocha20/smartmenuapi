using System.ComponentModel.DataAnnotations;

namespace SmartMenuManagerApp.Dto
{
    public class CreateMenuCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
