
namespace LeaveManagementSystem.Models
{
    public class LeaveBalance
    {
        public int LeaveBalanceId { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public int Year { get; set; }

        public int AllocatedDays { get; set; }

        public int UsedDays { get; set; }

        public int RemainingDays => AllocatedDays - UsedDays;

        public Employee Employee { get; set; } = null!;

        public LeaveType LeaveType { get; set; } = null!;
    }
}
