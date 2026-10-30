
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _repository;
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;

        public EmployeeService(
            EmployeeRepository repository,
            AppDbContext context,
            AuditLogService auditLogService)
        {
            _repository = repository;
            _context = context;
            _auditLogService = auditLogService;
        }

        // Get all employees
        public async Task<List<Employee>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Get employee by ID
        public async Task<Employee?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Employee ID must be positive.");

            return await _repository.GetByIdAsync(id);
        }

        // Add Employee
        public async Task AddAsync(Employee employee)
        {
            ValidateEmployee(employee);

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                await _repository.AddAsync(employee);

                _auditLogService.Log(
                    "Admin",
                    "Created",
                    "Employee",
                    employee.EmployeeId,
                    "New employee created",
                    null,
                    $"Name: {employee.FullName}, Email: {employee.Email}, Department: {employee.Department}"
                );

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Update Employee
        public async Task<bool> UpdateAsync(Employee employee)
        {
            ValidateEmployee(employee);

            if (employee.EmployeeId <= 0)
                throw new ArgumentException("Invalid employee ID.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var oldEmployee =
                    await _repository.GetByIdAsync(employee.EmployeeId);

                if (oldEmployee == null)
                    return false;

                string oldValues =
                    $"Name: {oldEmployee.FullName}, Email: {oldEmployee.Email}, Department: {oldEmployee.Department}";

                bool updated = await _repository.UpdateAsync(employee);

                if (!updated)
                    return false;

                string newValues =
                    $"Name: {employee.FullName}, Email: {employee.Email}, Department: {employee.Department}";

                _auditLogService.Log(
                    "Admin",
                    "Updated",
                    "Employee",
                    employee.EmployeeId,
                    "Employee details updated",
                    oldValues,
                    newValues
                );

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Delete Employee
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid employee ID.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var employee = await _repository.GetByIdAsync(id);

                if (employee == null)
                    return false;

                string oldValues =
                    $"Name: {employee.FullName}, Email: {employee.Email}, Department: {employee.Department}";

                bool deleted = await _repository.DeleteAsync(id);

                if (!deleted)
                    return false;

                _auditLogService.Log(
                    "Admin",
                    "Deleted",
                    "Employee",
                    id,
                    "Employee deleted",
                    oldValues,
                    null
                );

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static void ValidateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrWhiteSpace(employee.FullName))
                throw new ArgumentException("Employee name is required.");

            if (string.IsNullOrWhiteSpace(employee.Email) ||
                !employee.Email.Contains('@') ||
                !employee.Email.Contains('.'))
                throw new ArgumentException("Enter a valid email address.");

            if (string.IsNullOrWhiteSpace(employee.Department))
                throw new ArgumentException("Department is required.");

            if (employee.DateOfJoining.Date > DateTime.Today)
                throw new ArgumentException("Joining date cannot be in the future.");
        }
    }
}
