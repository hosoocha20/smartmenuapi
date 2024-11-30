namespace SmartMenuManagerApp.Dto
{
    public class ProductOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<OptionDetailDto> OptionDetails { get; set; } = new List<OptionDetailDto>();
    }
}
