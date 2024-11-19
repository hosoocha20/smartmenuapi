namespace SmartMenuManagerApp.Configurations
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }

        public ApiResponse(bool success, string message, string token = null)
        {
            Success = success;
            Message = message;
            Token = token;
        }
    }
}
