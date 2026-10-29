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

        public List<LeaveBalance> GetByEmployee(int employeeId, int year)
        {
            return _context.LeaveBalances
                .AsNoTracking()
                .Include(b => b.LeaveType)
                .Where(b => b.EmployeeId == employeeId && b.Year == year)
                .ToList();
        }

        public LeaveBalance? Get(int employeeId, int leaveTypeId, int year)
        {
            return _context.LeaveBalances.FirstOrDefault(b =>
                b.EmployeeId == employeeId &&
                b.LeaveTypeId == leaveTypeId &&
                b.Year == year);
        }

        public void Add(LeaveBalance balance)
        {
            _context.LeaveBalances.Add(balance);
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}