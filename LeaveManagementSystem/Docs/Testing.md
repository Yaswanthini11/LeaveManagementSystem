# Leave Management System — Testing

## 1. Testing Objective
Verify that employee management, leave type management, leave allocation, and leave request workflows function correctly.

## 2. Employee Management Tests

| Test | Expected Result |
|---|---|
| Add employee with valid details | Employee is saved |
| Enter an empty employee name | Validation error |
| Enter an invalid email | Validation error |
| View employees | Employee records are displayed |
| Update an existing employee | Changes are saved |
| Search for a nonexistent employee ID | Employee not found |
| Delete an employee without related records | Employee is deleted |

## 3. Leave Type Tests

| Test | Expected Result |
|---|---|
| Add a valid leave type | Leave type is saved |
| Enter an empty leave type name | Validation error |
| Enter zero or negative default days | Validation error |
| View leave types | Leave types are displayed |

## 4. Leave Balance Tests

| Test | Expected Result |
|---|---|
| Allocate balance for a valid employee and leave type | Balance is saved |
| Allocate a negative number of days | Validation error |
| View an employee's balance | Allocated, used, and remaining days are displayed |
| Allocate balance for the same employee, leave type, and year | Existing allocation is updated |

## 5. Leave Request Tests

| Test | Expected Result |
|---|---|
| Apply with valid details and sufficient balance | Request is created as Pending |
| Enter an end date before the start date | Request is rejected |
| Apply for leave in the past | Request is rejected |
| Apply without a reason | Request is rejected |
| Apply without an allocated balance | Request is rejected |
| Apply for more days than the available balance | Request is rejected |
| Apply for overlapping leave dates | Request is rejected |

## 6. Approval and Rejection Tests

| Test | Expected Result |
|---|---|
| Approve a pending request | Status becomes Approved |
| Approve the same request again | Operation is rejected |
| Approve a request with insufficient balance | Operation is rejected |
| Reject a pending request with a reason | Status becomes Rejected |
| Reject without a reason | Validation error |
| Reject an already processed request | Operation is rejected |
| Approve a request | Used leave days increase |
| Reject a request | Leave balance remains unchanged |

## 7. Database Verification
Verify that:
- Employee details are stored in Employees.
- Leave types are stored in LeaveTypes.
- Leave requests and their statuses are stored in LeaveRequests.
- Leave allocations and usage are stored in LeaveBalances.
- Foreign keys prevent invalid employee and leave type references.

## 8. Test Execution Record

| Test Date | Tester | Result | Remarks |
|---|---|---|---|
| To be completed | Developer | Pending | Record results after executing tests |

## 9. Conclusion
The application is considered tested only after the test cases have been executed and actual results match the expected results. Record any failures and resolve them before marking the project complete.
