
using System;
using System.Collections.Generic;

namespace LeaveManagementSystem.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public DateTime DateOfJoining { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<LeaveRequest> LeaveRequests { get; set; }
            = new List<LeaveRequest>();

        public ICollection<LeaveBalance> LeaveBalances { get; set; }
            = new List<LeaveBalance>();
    }
}
