namespace SmartMenuManagerApp.Configurations
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public double ExpirationInMinutes { get; set; }
    }
}
