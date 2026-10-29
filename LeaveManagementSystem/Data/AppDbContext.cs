
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LeaveManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<LeaveType> LeaveTypes { get; set; }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public DbSet<LeaveBalance> LeaveBalances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee configuration
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Leave type configuration
            modelBuilder.Entity<LeaveType>()
                .HasIndex(l => l.Name)
                .IsUnique();

            // Leave request relationships
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.LeaveType)
                .WithMany(lt => lt.LeaveRequests)
                .HasForeignKey(lr => lr.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Leave balance relationships
            modelBuilder.Entity<LeaveBalance>()
                .HasOne(lb => lb.Employee)
                .WithMany(e => e.LeaveBalances)
                .HasForeignKey(lb => lb.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveBalance>()
                .HasOne(lb => lb.LeaveType)
                .WithMany(lt => lt.LeaveBalances)
                .HasForeignKey(lb => lb.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // One balance per employee, leave type, and year
            modelBuilder.Entity<LeaveBalance>()
                .HasIndex(lb => new
                {
                    lb.EmployeeId,
                    lb.LeaveTypeId,
                    lb.Year
                })
                .IsUnique();

            // RemainingDays is calculated in C#, not stored in the database
            modelBuilder.Entity<LeaveBalance>()
                .Ignore(lb => lb.RemainingDays);

            // Basic string length constraints
            modelBuilder.Entity<Employee>()
                .Property(e => e.FullName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Employee>()
                .Property(e => e.Email)
                .HasMaxLength(150)
                .IsRequired();

            modelBuilder.Entity<LeaveType>()
                .Property(lt => lt.Name)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.Reason)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.Status)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<LeaveRequest>()
                .Property(lr => lr.RejectionReason)
                .HasMaxLength(500);
        }
    }
}
