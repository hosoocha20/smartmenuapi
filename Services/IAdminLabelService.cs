using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Services
{
    public interface IAdminLabelService
    {
        Task<Label> AddLabelAsync(CreateLabelDto labelDto);
    }
}
