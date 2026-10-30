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

        public List<AuditLog> GetAllLogs()
        {
            return _repository.GetAll();
        }
    }
}