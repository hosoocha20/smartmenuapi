namespace SmartMenuManagerApp.Dto
{
    public class CreateProductOptionDto
    {
        public string Name { get; set; }
        public List<CreateOptionDetailDto> OptionDetails { get; set; } = new List<CreateOptionDetailDto>();
    }
}
