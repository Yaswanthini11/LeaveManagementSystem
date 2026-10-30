namespace LeaveManagementSystem.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public string PerformedBy { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public string Module { get; set; } = string.Empty;

        public int? RecordId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}