using SmartMenuManagerApp.Data;
using SmartMenuManagerApp.Interfaces;
using SmartMenuManagerApp.Models;

namespace SmartMenuManagerApp.Repository
{
    public class AdminLabelRepository : IAdminLabelRepository
    {
        private readonly DataContext _context;

        public AdminLabelRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Label label)
        {
            await _context.Labels.AddAsync(label);
        }

        public async Task DeleteAsync(Label label)
        {
            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();
        }

        public async Task<Label> GetByIdAsync(int id)
        {
            return await _context.Labels.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
