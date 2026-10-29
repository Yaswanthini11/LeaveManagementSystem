
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories
{
    public class EmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all employees
        public List<Employee> GetAll()
        {
            return _context.Employees
                .AsNoTracking()
                .ToList();
        }

        // Get employee by ID
        public Employee? GetById(int employeeId)
        {
            return _context.Employees
                .AsNoTracking()
                .FirstOrDefault(e => e.EmployeeId == employeeId);
        }

        // Add employee
        public void Add(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        // Update employee
        public bool Update(Employee employee)
        {
            var existingEmployee = _context.Employees
                .Find(employee.EmployeeId);

            if (existingEmployee == null)
            {
                return false;
            }

            existingEmployee.FullName = employee.FullName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Department = employee.Department;
            existingEmployee.DateOfJoining = employee.DateOfJoining;
            existingEmployee.IsActive = employee.IsActive;

            _context.SaveChanges();
            return true;
        }

        // Delete employee
        public bool Delete(int employeeId)
        {
            var employee = _context.Employees.Find(employeeId);

            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return true;
        }
    }
}
