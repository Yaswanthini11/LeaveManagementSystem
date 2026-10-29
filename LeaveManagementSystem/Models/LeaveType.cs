
using System.Collections.Generic;

namespace LeaveManagementSystem.Models
{
    public class LeaveType
    {
        public int LeaveTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DefaultDays { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<LeaveRequest> LeaveRequests { get; set; }
            = new List<LeaveRequest>();

        public ICollection<LeaveBalance> LeaveBalances { get; set; }
            = new List<LeaveBalance>();
    }
}
