using SmartMenuManagerApp.Dto;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Services
{
    public class AdminLabelService : IAdminLabelService
    {
        readonly IAdminLabelRepository _adminLabelRepository;

        public AdminLabelService(IAdminLabelRepository adminLabelRepository)
        {
            _adminLabelRepository = adminLabelRepository;
        }

        public async Task<Label> AddLabelAsync(CreateLabelDto request)
        {
            var label = new Label
            {
                Name = request.Name,
            };
            await _adminLabelRepository.AddAsync(label);
            await _adminLabelRepository.SaveChangesAsync();
            return label;
        }
    }
}
