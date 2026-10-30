using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories
{
    public class AuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(AuditLog auditLog)
        {
            _context.AuditLogs.Add(auditLog);
        }

        public List<AuditLog> GetAll()
        {
            return _context.AuditLogs
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }
    }
}
