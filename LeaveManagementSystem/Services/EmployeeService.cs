using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Services
{
    public class EmployeeService
    {
        private readonly EmployeeRepository _repository;

        public EmployeeService(EmployeeRepository repository)
        {
            _repository = repository;
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

        public void Add(Employee employee)
        {
            ValidateEmployee(employee);
            _repository.Add(employee);
        }

        public bool Update(Employee employee)
        {
            ValidateEmployee(employee);

            if (employee.EmployeeId <= 0)
                throw new ArgumentException("Invalid employee ID.");

            return _repository.Update(employee);
        }

        public bool Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid employee ID.");

            return _repository.Delete(id);
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