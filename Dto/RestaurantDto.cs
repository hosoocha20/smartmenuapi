using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Dto
{
    public class RestaurantDto
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public string Address { get; set; } 
        // Opening and closing times
        public TimeSpan OpeningTime { get; set; } 
        public TimeSpan ClosingTime { get; set; } 
        public string PosProvider { get; set; }


        public List<MyTableDto> MyTables { get; set; } = new List<MyTableDto>();  // Tables in the restaurant
        public MenuDto Menu { get; set; }  // Restaurant's menu
        public ThemeDto Theme { get; set; }  // Customizable theme
    }
}
