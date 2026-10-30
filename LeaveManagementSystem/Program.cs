
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;
using LeaveManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LeaveManagementSystem
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            
             var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

            string connectionString =
                configuration.GetConnectionString("DefaultConnection")!;

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;
            using var context = new AppDbContext(options);

            // Repositories
            var employeeRepository = new EmployeeRepository(context);
            var leaveTypeRepository = new LeaveTypeRepository(context);
            var requestRepository = new LeaveRequestRepository(context);
            var balanceRepository = new LeaveBalanceRepository(context);
            var auditLogRepository = new AuditLogRepository(context);

            // Audit service
            var auditLogService = new AuditLogService(auditLogRepository);

            // Main services
            var employeeService = new EmployeeService(
                employeeRepository,
                context,
                auditLogService);

            var leaveTypeService = new LeaveTypeService(
                leaveTypeRepository,
                context,
                auditLogService);

            var balanceService = new LeaveBalanceService(
                balanceRepository,
                context,
                auditLogService);

            var requestService = new LeaveRequestService(
                requestRepository,
                employeeRepository,
                leaveTypeRepository,
                balanceRepository,
                context,
                auditLogService);

            while (true)
            {
                Console.WriteLine("\n===== LEAVE MANAGEMENT SYSTEM =====");
                Console.WriteLine("1. Add Employee");
                Console.WriteLine("2. View Employees");
                Console.WriteLine("3. Update Employee");
                Console.WriteLine("4. Delete Employee");
                Console.WriteLine("5. Add Leave Type");
                Console.WriteLine("6. View Leave Types");
                Console.WriteLine("7. Allocate Leave Balance");
                Console.WriteLine("8. View Employee Leave Balance");
                Console.WriteLine("9. Apply for Leave");
                Console.WriteLine("10. View Leave Requests");
                Console.WriteLine("11. Approve Leave");
                Console.WriteLine("12. Reject Leave");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await employeeService.AddAsync(new Employee
                            {
                                FullName = ReadText("Full name: "),
                                Email = ReadText("Email: "),
                                Department = ReadText("Department: "),
                                DateOfJoining = ReadDate("Date of joining (yyyy-MM-dd): ")
                            });

                            Console.WriteLine("Employee added.");
                            break;

                        case "2":
                            var employees = await employeeService.GetAllAsync();

                            foreach (var e in employees)
                            {
                                Console.WriteLine(
                                    $"{e.EmployeeId} | {e.FullName} | {e.Email} | {e.Department} | Active: {e.IsActive}");
                            }
                            break;

                        case "3":
                            int updateId = ReadInt("Employee ID: ");

                            var employee =
                                await employeeService.GetByIdAsync(updateId);

                            if (employee == null)
                            {
                                Console.WriteLine("Employee not found.");
                                break;
                            }

                            employee.FullName = ReadText("New full name: ");
                            employee.Email = ReadText("New email: ");
                            employee.Department = ReadText("New department: ");
                            employee.DateOfJoining = ReadDate("New joining date (yyyy-MM-dd): ");

                            bool employeeUpdated =
                                await employeeService.UpdateAsync(employee);

                            Console.WriteLine(employeeUpdated
                                ? "Employee updated."
                                : "Employee not found.");
                            break;

                        case "4":
                            bool employeeDeleted =
                                await employeeService.DeleteAsync(
                                    ReadInt("Employee ID: "));

                            Console.WriteLine(employeeDeleted
                                ? "Employee deleted."
                                : "Employee not found.");
                            break;

                        case "5":
                            await leaveTypeService.AddAsync(new LeaveType
                            {
                                Name = ReadText("Leave type name: "),
                                DefaultDays = ReadInt("Default days: ")
                            });

                            Console.WriteLine("Leave type added.");
                            break;

                        case "6":
                            var leaveTypes =
                                await leaveTypeService.GetAllAsync();

                            foreach (var t in leaveTypes)
                            {
                                Console.WriteLine(
                                    $"{t.LeaveTypeId} | {t.Name} | {t.DefaultDays} days | Active: {t.IsActive}");
                            }
                            break;

                        case "7":
                            await balanceService.AllocateAsync(
                                ReadInt("Employee ID: "),
                                ReadInt("Leave type ID: "),
                                ReadInt("Year: "),
                                ReadInt("Allocated days: "));

                            Console.WriteLine("Leave balance allocated.");
                            break;

                        case "8":
                            int balanceEmployeeId =
                                ReadInt("Employee ID: ");

                            int year = ReadInt("Year: ");

                            var balances =
                                await balanceService.GetByEmployeeAsync(
                                    balanceEmployeeId, year);

                            foreach (var b in balances)
                            {
                                Console.WriteLine(
                                    $"{b.LeaveType.Name}: allocated={b.AllocatedDays}, used={b.UsedDays}, remaining={b.RemainingDays}");
                            }
                            break;

                        case "9":
                            await requestService.ApplyAsync(
                                ReadInt("Employee ID: "),
                                ReadInt("Leave type ID: "),
                                ReadDate("Start date (yyyy-MM-dd): "),
                                ReadDate("End date (yyyy-MM-dd): "),
                                ReadText("Reason: "));

                            Console.WriteLine("Leave request submitted.");
                            break;

                        case "10":
                            var requests =
                                await requestService.GetAllAsync();

                            foreach (var r in requests)
                            {
                                Console.WriteLine(
                                    $"{r.LeaveRequestId} | {r.Employee.FullName} | {r.LeaveType.Name} | {r.StartDate:yyyy-MM-dd} to {r.EndDate:yyyy-MM-dd} | {r.TotalDays} days | {r.Status} | {r.RejectionReason}");
                            }
                            break;

                        case "11":
                            await requestService.ApproveAsync(
                                ReadInt("Leave request ID: "));

                            Console.WriteLine("Leave request approved.");
                            break;

                        case "12":
                            await requestService.RejectAsync(
                                ReadInt("Leave request ID: "),
                                ReadText("Rejection reason: "));

                            Console.WriteLine("Leave request rejected.");
                            break;

                        case "0":
                            Console.WriteLine("Exiting Leave Management System.");
                            return;

                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static string ReadText(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;

                Console.WriteLine("Enter a valid number.");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                    return date;

                Console.WriteLine("Enter a valid date.");
            }
        }
    }
}
