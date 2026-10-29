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

        public List<LeaveType> GetAll()
        {
            return _context.LeaveTypes
                .AsNoTracking()
                .ToList();
        }

        public LeaveType? GetById(int id)
        {
            return _context.LeaveTypes.Find(id);
        }

        public void Add(LeaveType leaveType)
        {
            _context.LeaveTypes.Add(leaveType);
            _context.SaveChanges();
        }

        public bool Update(LeaveType leaveType)
        {
            var existing = _context.LeaveTypes
                .Find(leaveType.LeaveTypeId);

            if (existing == null)
                return false;

            existing.Name = leaveType.Name;
            existing.DefaultDays = leaveType.DefaultDays;
            existing.IsActive = leaveType.IsActive;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var existing = _context.LeaveTypes.Find(id);

            if (existing == null)
                return false;

            _context.LeaveTypes.Remove(existing);
            _context.SaveChanges();
            return true;
        }
    }
}