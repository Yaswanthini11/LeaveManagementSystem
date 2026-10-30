
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
        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }

        // Get employee by ID
        public async Task<Employee?> GetById(int employeeId)
        {
            return await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        // Add employee
        public async Task Add(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        // Update employee
        public async Task<bool> Update(Employee employee)
        {
            var existingEmployee = await _context.Employees
                .FindAsync(employee.EmployeeId);

            if (existingEmployee == null)
            {
                return false;
            }

            existingEmployee.FullName = employee.FullName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Department = employee.Department;
            existingEmployee.DateOfJoining = employee.DateOfJoining;
            existingEmployee.IsActive = employee.IsActive;

            await _context.SaveChangesAsync();

            return true;
        }

        // Delete employee
        public async Task<bool> Delete(int employeeId)
        {
            var employee = await _context.Employees
                .FindAsync(employeeId);

            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
