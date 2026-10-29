using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Services
{
    public class LeaveBalanceService
    {
        private readonly LeaveBalanceRepository _repository;

        public LeaveBalanceService(LeaveBalanceRepository repository)
        {
            _repository = repository;
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

            var existing = _repository.Get(employeeId, leaveTypeId, year);

            if (existing != null)
            {
                existing.AllocatedDays = allocatedDays;
                _repository.SaveChanges();
                return;
            }

            _repository.Add(new LeaveBalance
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                Year = year,
                AllocatedDays = allocatedDays,
                UsedDays = 0
            });
        }
    }
}