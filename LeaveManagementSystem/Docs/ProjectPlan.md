# Leave Management System — Project Plan

## 1. Project Overview
The Leave Management System is a C# console application that manages employees, leave types, leave applications, approvals, and leave balances. It uses Entity Framework Core and SQL Server Express for database operations.

## 2. Project Objectives
- Maintain employee information.
- Manage different leave types.
- Allow employees to apply for leave.
- Validate leave dates and available balance.
- Approve or reject leave requests.
- Track allocated, used, and remaining leave days.
- Maintain leave request history.

## 3. Technologies Used
- C#
- .NET 10
- Entity Framework Core
- SQL Server Express
- Git and GitHub
- Visual Studio

## 4. Project Architecture
- **Models:** Represent employees, leave types, leave requests, and leave balances.
- **Data:** Contains the database context and configuration.
- **Repositories:** Handle database CRUD operations.
- **Services:** Implement validation and business rules.
- **Helpers:** Provide reusable input and validation methods.
- **Docs:** Stores project planning, database setup, development, and testing documentation.

## 5. Main Modules
1. Employee Management
2. Leave Type Management
3. Leave Balance Allocation
4. Leave Application
5. Leave Approval and Rejection
6. Leave Balance and Request History

## 6. Database Tables
- Employees
- LeaveTypes
- LeaveRequests
- LeaveBalances

## 7. Development Phases
1. Create the project and configure GitHub.
2. Define models and database relationships.
3. Configure Entity Framework Core and migrations.
4. Implement repositories and services.
5. Implement the console menu.
6. Test the application and database operations.
7. Complete documentation and publish the code to GitHub.

## 8. Expected Outcome
A console-based application that simplifies leave management and maintains leave records in SQL Server.
