
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories
{
    public class LeaveBalanceRepository
    {
        private readonly AppDbContext _context;

        public LeaveBalanceRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get leave balances by employee and year
        public async Task<List<LeaveBalance>> GetByEmployeeAsync(
            int employeeId, int year)
        {
            return await _context.LeaveBalances
                .AsNoTracking()
                .Include(b => b.LeaveType)
                .Where(b => b.EmployeeId == employeeId && b.Year == year)
                .ToListAsync();
        }

        // Get a specific leave balance
        public async Task<LeaveBalance?> GetAsync(
            int employeeId, int leaveTypeId, int year)
        {
            return await _context.LeaveBalances
                .FirstOrDefaultAsync(b =>
                    b.EmployeeId == employeeId &&
                    b.LeaveTypeId == leaveTypeId &&
                    b.Year == year);
        }

        // Add leave balance
        public async Task AddAsync(LeaveBalance balance)
        {
            _context.LeaveBalances.Add(balance);
            await _context.SaveChangesAsync();
        }

        // Save changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
