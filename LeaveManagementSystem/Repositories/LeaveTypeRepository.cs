
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveTypeRepository
    {
        private readonly AppDbContext _context;

        public LeaveTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all leave types
        public async Task<List<LeaveType>> GetAllAsync()
        {
            return await _context.LeaveTypes
                .AsNoTracking()
                .ToListAsync();
        }

        // Get leave type by ID
        public async Task<LeaveType?> GetByIdAsync(int id)
        {
            return await _context.LeaveTypes
                .FindAsync(id);
        }

        // Add leave type
        public async Task AddAsync(LeaveType leaveType)
        {
            _context.LeaveTypes.Add(leaveType);
            await _context.SaveChangesAsync();
        }

        // Update leave type
        public async Task<bool> UpdateAsync(LeaveType leaveType)
        {
            var existing = await _context.LeaveTypes
                .FindAsync(leaveType.LeaveTypeId);

            if (existing == null)
            {
                return false;
            }

            existing.Name = leaveType.Name;
            existing.DefaultDays = leaveType.DefaultDays;
            existing.IsActive = leaveType.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        // Delete leave type
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.LeaveTypes
                .FindAsync(id);

            if (existing == null)
            {
                return false;
            }

            _context.LeaveTypes.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
