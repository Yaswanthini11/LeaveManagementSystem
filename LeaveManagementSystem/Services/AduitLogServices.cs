
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Services
{
    public class AuditLogService
    {
        private readonly AuditLogRepository _repository;

        public AuditLogService(AuditLogRepository repository)
        {
            _repository = repository;
        }

        // Add audit log
        public void Log(
            string performedBy,
            string action,
            string module,
            int? recordId,
            string description,
            string? oldValues = null,
            string? newValues = null)
        {
            var auditLog = new AuditLog
            {
                PerformedBy = performedBy,
                Action = action,
                Module = module,
                RecordId = recordId,
                Description = description,
                OldValues = oldValues,
                NewValues = newValues,
                CreatedAt = DateTime.Now
            };

            _repository.Add(auditLog);
        }

        // Get all audit logs
        public async Task<List<AuditLog>> GetAllLogsAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
