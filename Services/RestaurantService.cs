using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Services
{
    public class RestaurantService:IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        public async Task<RestaurantDto> GetRestaurantAsync(string userId)
        {
            var restaurantCheck = await _restaurantRepository.GetByUserIdAsync(userId);

            if (restaurantCheck == null)
                throw new UnauthorizedAccessException("You do not have access to this restaurant.");

            var restaurant = await _restaurantRepository.GetRestaurantWithDetailsAsync(restaurantCheck.Id);

            if (restaurant == null)
                return null;

            return new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Address = restaurant.Address,
                OpeningTime = restaurant.OpeningTime,
                ClosingTime = restaurant.ClosingTime,
                PosProvider = restaurant.PosProvider,
                MyTables = restaurant.MyTables.Select(t => new MyTableDto { Id = t.Id, Code = t.Code }).ToList(),
                Menu = new MenuDto
                {
                    Id = restaurant.Menu.Id,
                    Name = restaurant.Menu.Name,
                    Categories = restaurant.Menu.MenuCategories.Select(c => new MenuCategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        SubCategories = c.MenuSubCategories.Select(sc => new MenuSubCategoryDto
                        {
                            Id = sc.Id,
                            Name = sc.Name,
                            Products = sc.Products.Select(p => new ProductDto
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Price = p.Price,
                                ImgUrl = p.ImgUrl,
                                Labels = p.ProductLabels.Select(pl => new LabelDto
                                {
                                    Id = pl.Label.Id,
                                    Name = pl.Label.Name,
                                }).ToList(),
                                Options = p.ProductOptions.Select(po => new ProductOptionDto
                                {
                                    Id = po.Id,
                                    Name = po.Name,
                                    OptionDetails = po.OptionDetails.Select(od => new OptionDetailDto
                                    {
                                        Id = od.Id,
                                        Name = od.Name,
                                        AdditionalPrice = od.AdditionalPrice
                                    }).ToList()
                                }).ToList()
                            }).ToList()
                        }).ToList(),
                        Products = c.Products.Where(p => !c.MenuSubCategories.Any(sc => sc.Products.Contains(p))) // Exclude products that are already in a subcategory
                                         .Select(p => new ProductDto
                                         {
                                             Id = p.Id,
                                             Name = p.Name,
                                             Description = p.Description,
                                             Price = p.Price,
                                             ImgUrl = p.ImgUrl,
                                             Labels = p.ProductLabels.Select(pl => new LabelDto
                                             {
                                                 Id = pl.Label.Id,
                                                 Name = pl.Label.Name,
                                             }).ToList(),
                                             Options = p.ProductOptions.Select(po => new ProductOptionDto
                                             {
                                                 Id = po.Id,
                                                 Name = po.Name,
                                                 OptionDetails = po.OptionDetails.Select(od => new OptionDetailDto
                                                 {
                                                     Id = od.Id,
                                                     Name = od.Name,
                                                     AdditionalPrice = od.AdditionalPrice
                                                 }).ToList()
                                             }).ToList()
                                         }).ToList()
                    }).ToList()
                }, 
                Theme = new ThemeDto
                {
                    Id = restaurant.Theme.Id,
                    HeaderColor = restaurant.Theme.HeaderColor,
                    HeaderTextColor = restaurant.Theme.HeaderTextColor,
                    SubHeaderImgUrl = restaurant.Theme.SubHeaderImgUrl,
                    SidebarColor = restaurant.Theme.SidebarColor,
                    SidebarTextColor = restaurant.Theme.SidebarTextColor,
                    SidebarSelectedColor = restaurant.Theme.SidebarSelectedColor,
                    BodyColor = restaurant.Theme.BodyColor,
                    BodyHeaderTextColor = restaurant.Theme.BodyHeaderTextColor,
                    MenuItemBodyColor = restaurant.Theme.MenuItemBodyColor,
                    MenuItemTextColor = restaurant.Theme.MenuItemTextColor,
                    MenuItemPriceColor = restaurant.Theme.MenuItemPriceColor,
                    ButtonColor = restaurant.Theme.ButtonColor,
                    ButtonTextColor = restaurant.Theme.ButtonTextColor,
                    SubCategoryTextColor = restaurant.Theme.SubCategoryTextColor,
                    LogoUrl = restaurant.Theme.LogoUrl
                }
            };
        }


    }
}
