
using LeaveManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(
                    @"Server=.\SQLEXPRESS;Database=LeaveManagementDb;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            using var context = new AppDbContext(options);

            Console.WriteLine("Leave Management System");
            Console.WriteLine("Database configuration completed!");
        }
    }
}
