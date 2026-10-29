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

        public List<LeaveRequest> GetAll()
        {
            return _context.LeaveRequests
                .AsNoTracking()
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .OrderByDescending(r => r.AppliedOn)
                .ToList();
        }

        public LeaveRequest? GetById(int id)
        {
            return _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.LeaveType)
                .FirstOrDefault(r => r.LeaveRequestId == id);
        }

        public void Add(LeaveRequest request)
        {
            _context.LeaveRequests.Add(request);
            _context.SaveChanges();
        }

        public bool Update(LeaveRequest request)
        {
            var existing = _context.LeaveRequests
                .Find(request.LeaveRequestId);

            if (existing == null)
                return false;

            existing.Status = request.Status;
            existing.RejectionReason = request.RejectionReason;

            _context.SaveChanges();
            return true;
        }
    }
}