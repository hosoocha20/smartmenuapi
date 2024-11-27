namespace SmartMenuManagerApp.Dto
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RestaurantName { get; set; }  // The name of the restaurant
        public string Address { get; set; }
        public string PhoneNumber {  get; set; }
        public string PosProvider {  get; set; }
        public TimeSpan OpeningTime { get; set; } // Store opening time
        public TimeSpan ClosingTime { get; set; } // Store closing time
        public string Role { get; set; } = "User";
    }
}
