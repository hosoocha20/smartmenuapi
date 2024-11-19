namespace SmartMenuManagerApp.Models
{
    public class Theme
    {
        public int Id { get; set; } // Primary Key
        public string HeaderColor { get; set; } // Header Background color
        public string HeaderTextColor { get; set; } //Header Text Color
        public string? SubHeaderImgUrl { get; set; } //Sub Header/Banner image - for branding - this could be null - if none will use defauly image banner
        public string SidebarColor { get; set; } //Side menu bar bg color
        public string SidebarTextColor { get; set; }
        public string SidebarSelectedColor { get; set; }
        public string BodyColor { get; set; } // Body Color
        public string BodyHeaderTextColor { get; set; } // 
        public string MenuItemBodyColor { get; set; }
        public string MenuItemTextColor { get; set; }
        public string MenuItemPriceColor { get; set; }
        public string ButtonColor { get; set; }
        public string ButtonTextColor { get; set; }
        public string SubCategoryTextColor { get; set; }
        public string LogoUrl { get; set; } // URL to the restaurant's logo image

        // Foreign Key
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } // Navigation property to Restaurant
    }
}
