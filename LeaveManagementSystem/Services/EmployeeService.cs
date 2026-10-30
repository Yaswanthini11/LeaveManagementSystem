
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

        public List<Employee> GetAll()
        {
            return _repository.GetAll();
        }

        public Employee? GetById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Employee ID must be positive.");

            return _repository.GetById(id);
        }

        // Add Employee
        public void Add(Employee employee)
        {
            ValidateEmployee(employee);

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _repository.Add(employee);

                _auditLogService.Log(
                    "Admin",
                    "Created",
                    "Employee",
                    employee.EmployeeId,
                    "New employee created",
                    null,
                    $"Name: {employee.FullName}, Email: {employee.Email}, Department: {employee.Department}"
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

        // Update Employee
        public bool Update(Employee employee)
        {
            ValidateEmployee(employee);

            if (employee.EmployeeId <= 0)
                throw new ArgumentException("Invalid employee ID.");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var oldEmployee = _repository.GetById(employee.EmployeeId);

                if (oldEmployee == null)
                    return false;

                string oldValues =
                    $"Name: {oldEmployee.FullName}, Email: {oldEmployee.Email}, Department: {oldEmployee.Department}";

                bool updated = _repository.Update(employee);

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

                _context.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Delete Employee
        public bool Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid employee ID.");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var employee = _repository.GetById(id);

                if (employee == null)
                    return false;

                string oldValues =
                    $"Name: {employee.FullName}, Email: {employee.Email}, Department: {employee.Department}";

                bool deleted = _repository.Delete(id);

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

                _context.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
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
