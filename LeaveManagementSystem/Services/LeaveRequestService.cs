
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Services
{
    public class LeaveRequestService
    {
        private readonly LeaveRequestRepository _requestRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly LeaveTypeRepository _leaveTypeRepository;
        private readonly LeaveBalanceRepository _balanceRepository;
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;

        public LeaveRequestService(
            LeaveRequestRepository requestRepository,
            EmployeeRepository employeeRepository,
            LeaveTypeRepository leaveTypeRepository,
            LeaveBalanceRepository balanceRepository,
            AppDbContext context,
            AuditLogService auditLogService)
        {
            _requestRepository = requestRepository;
            _employeeRepository = employeeRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _balanceRepository = balanceRepository;
            _context = context;
            _auditLogService = auditLogService;
        }

        // Get all leave requests
        public async Task<List<LeaveRequest>> GetAllAsync()
        {
            return await _requestRepository.GetAllAsync();
        }

        // Apply for Leave
        public async Task ApplyAsync(
            int employeeId,
            int leaveTypeId,
            DateTime startDate,
            DateTime endDate,
            string reason)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(employeeId);

            var leaveType =
                await _leaveTypeRepository.GetByIdAsync(leaveTypeId);

            if (employee == null || !employee.IsActive)
                throw new ArgumentException("Employee not found or inactive.");

            if (leaveType == null || !leaveType.IsActive)
                throw new ArgumentException("Leave type not found or inactive.");

            if (startDate.Date < DateTime.Today)
                throw new ArgumentException("Leave cannot start in the past.");

            if (endDate.Date < startDate.Date)
                throw new ArgumentException("End date cannot be before start date.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason is required.");

            if (reason.Length > 500)
                throw new ArgumentException("Reason cannot exceed 500 characters.");

            if (startDate.Year != endDate.Year)
                throw new ArgumentException("Please apply separately for leave across different years.");

            int totalDays = (endDate.Date - startDate.Date).Days + 1;

            var balance = await _balanceRepository.GetAsync(
                employeeId, leaveTypeId, startDate.Year);

            if (balance == null)
                throw new ArgumentException("Leave balance has not been allocated.");

            if (balance.RemainingDays < totalDays)
                throw new ArgumentException("Insufficient leave balance.");

            var existingRequests =
                await _requestRepository.GetAllAsync();

            bool overlaps = existingRequests.Any(r =>
                r.EmployeeId == employeeId &&
                r.Status != "Rejected" &&
                r.StartDate.Date <= endDate.Date &&
                r.EndDate.Date >= startDate.Date);

            if (overlaps)
                throw new ArgumentException("This leave period overlaps another request.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var request = new LeaveRequest
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = leaveTypeId,
                    StartDate = startDate.Date,
                    EndDate = endDate.Date,
                    TotalDays = totalDays,
                    Reason = reason.Trim(),
                    Status = "Pending",
                    AppliedOn = DateTime.Now
                };

                await _requestRepository.AddAsync(request);

                _auditLogService.Log(
                    "Employee",
                    "Applied",
                    "LeaveRequest",
                    request.LeaveRequestId,
                    "New leave request submitted",
                    null,
                    $"Employee ID: {employeeId}, Leave Type ID: {leaveTypeId}, Total Days: {totalDays}, Status: Pending"
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

        // Approve Leave
        public async Task ApproveAsync(int requestId)
        {
            var request =
                await _requestRepository.GetByIdAsync(requestId);

            if (request == null)
                throw new ArgumentException("Leave request not found.");

            if (request.Status != "Pending")
                throw new ArgumentException("Only pending requests can be approved.");

            var balance = await _balanceRepository.GetAsync(
                request.EmployeeId,
                request.LeaveTypeId,
                request.StartDate.Year);

            if (balance == null || balance.RemainingDays < request.TotalDays)
                throw new ArgumentException("Insufficient leave balance.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                string oldValues =
                    $"Status: {request.Status}, Used Days: {balance.UsedDays}";

                balance.UsedDays += request.TotalDays;
                request.Status = "Approved";

                await _requestRepository.UpdateAsync(request);
                await _balanceRepository.SaveChangesAsync();

                string newValues =
                    $"Status: {request.Status}, Used Days: {balance.UsedDays}";

                _auditLogService.Log(
                    "Admin",
                    "Approved",
                    "LeaveRequest",
                    request.LeaveRequestId,
                    "Leave request approved and leave balance updated",
                    oldValues,
                    newValues
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

        // Reject Leave
        public async Task RejectAsync(
            int requestId, string rejectionReason)
        {
            var request =
                await _requestRepository.GetByIdAsync(requestId);

            if (request == null)
                throw new ArgumentException("Leave request not found.");

            if (request.Status != "Pending")
                throw new ArgumentException("Only pending requests can be rejected.");

            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new ArgumentException("Rejection reason is required.");

            if (rejectionReason.Length > 500)
                throw new ArgumentException("Rejection reason cannot exceed 500 characters.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                string oldValues = $"Status: {request.Status}";

                request.Status = "Rejected";
                request.RejectionReason = rejectionReason.Trim();

                await _requestRepository.UpdateAsync(request);

                string newValues =
                    $"Status: {request.Status}, Rejection Reason: {request.RejectionReason}";

                _auditLogService.Log(
                    "Admin",
                    "Rejected",
                    "LeaveRequest",
                    request.LeaveRequestId,
                    "Leave request rejected",
                    oldValues,
                    newValues
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
    }
}
