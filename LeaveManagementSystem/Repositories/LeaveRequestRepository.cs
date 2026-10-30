
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveRequestRepository
    {
        private readonly AppDbContext _context;

        public LeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all leave requests
        public async Task<List<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests
                .AsNoTracking()
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .OrderByDescending(r => r.AppliedOn)
                .ToListAsync();
        }

        // Get leave request by ID
        public async Task<LeaveRequest?> GetByIdAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .FirstOrDefaultAsync(r => r.LeaveRequestId == id);
        }

        // Add leave request
        public async Task AddAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        // Update leave request
        public async Task<bool> UpdateAsync(LeaveRequest request)
        {
            var existing = await _context.LeaveRequests
                .FindAsync(request.LeaveRequestId);

            if (existing == null)
            {
                return false;
            }

            existing.Status = request.Status;
            existing.RejectionReason = request.RejectionReason;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
