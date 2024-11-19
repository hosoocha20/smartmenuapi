using SmartMenuManagerApp.Configurations;
using SmartMenuManagerApp.Dto;

namespace SmartMenuManagerApp.Authentication
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse> LoginAsync(LoginDto loginDto);
    }
}
