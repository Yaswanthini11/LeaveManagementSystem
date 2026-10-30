
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Services
{
    public class LeaveBalanceService
    {
        private readonly LeaveBalanceRepository _repository;
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;

        public LeaveBalanceService(
            LeaveBalanceRepository repository,
            AppDbContext context,
            AuditLogService auditLogService)
        {
            _repository = repository;
            _context = context;
            _auditLogService = auditLogService;
        }

        public List<LeaveBalance> GetByEmployee(int employeeId, int year)
        {
            return _repository.GetByEmployee(employeeId, year);
        }

        public void Allocate(
            int employeeId,
            int leaveTypeId,
            int year,
            int allocatedDays)
        {
            if (employeeId <= 0 || leaveTypeId <= 0)
                throw new ArgumentException("Invalid employee or leave type ID.");

            if (year < 2000 || year > 2100)
                throw new ArgumentException("Invalid year.");

            if (allocatedDays < 0)
                throw new ArgumentException("Allocated days cannot be negative.");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var existing = _repository.Get(employeeId, leaveTypeId, year);

                string? oldValues = null;
                string action;

                LeaveBalance balance;

                if (existing != null)
                {
                    oldValues =
                        $"Allocated Days: {existing.AllocatedDays}, Used Days: {existing.UsedDays}";

                    existing.AllocatedDays = allocatedDays;

                    balance = existing;
                    action = "Updated";

                    _repository.SaveChanges();
                }
                else
                {
                    balance = new LeaveBalance
                    {
                        EmployeeId = employeeId,
                        LeaveTypeId = leaveTypeId,
                        Year = year,
                        AllocatedDays = allocatedDays,
                        UsedDays = 0
                    };

                    _repository.Add(balance);
                    action = "Allocated";
                }

                string newValues =
                    $"Allocated Days: {balance.AllocatedDays}, Used Days: {balance.UsedDays}";

                _auditLogService.Log(
                    "Admin",
                    action,
                    "LeaveBalance",
                    balance.LeaveBalanceId,
                    "Employee leave balance allocated or updated",
                    oldValues,
                    newValues
                );

                _context.SaveChanges();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
