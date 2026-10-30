
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

        public List<LeaveRequest> GetAll()
        {
            return _requestRepository.GetAll();
        }

        // Apply for Leave
        public void Apply(
            int employeeId,
            int leaveTypeId,
            DateTime startDate,
            DateTime endDate,
            string reason)
        {
            var employee = _employeeRepository.GetById(employeeId);
            var leaveType = _leaveTypeRepository.GetById(leaveTypeId);

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

            var balance = _balanceRepository.Get(
                employeeId, leaveTypeId, startDate.Year);

            if (balance == null)
                throw new ArgumentException("Leave balance has not been allocated.");

            if (balance.RemainingDays < totalDays)
                throw new ArgumentException("Insufficient leave balance.");

            var existingRequests = _requestRepository.GetAll();

            bool overlaps = existingRequests.Any(r =>
                r.EmployeeId == employeeId &&
                r.Status != "Rejected" &&
                r.StartDate.Date <= endDate.Date &&
                r.EndDate.Date >= startDate.Date);

            if (overlaps)
                throw new ArgumentException("This leave period overlaps another request.");

            using var transaction = _context.Database.BeginTransaction();

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

                _requestRepository.Add(request);

                _auditLogService.Log(
                    "Employee",
                    "Applied",
                    "LeaveRequest",
                    request.LeaveRequestId,
                    "New leave request submitted",
                    null,
                    $"Employee ID: {employeeId}, Leave Type ID: {leaveTypeId}, Total Days: {totalDays}, Status: Pending"
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

        // Approve Leave
        public void Approve(int requestId)
        {
            var request = _requestRepository.GetById(requestId);

            if (request == null)
                throw new ArgumentException("Leave request not found.");

            if (request.Status != "Pending")
                throw new ArgumentException("Only pending requests can be approved.");

            var balance = _balanceRepository.Get(
                request.EmployeeId,
                request.LeaveTypeId,
                request.StartDate.Year);

            if (balance == null || balance.RemainingDays < request.TotalDays)
                throw new ArgumentException("Insufficient leave balance.");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                string oldValues =
                    $"Status: {request.Status}, Used Days: {balance.UsedDays}";

                balance.UsedDays += request.TotalDays;
                request.Status = "Approved";

                _requestRepository.Update(request);
                _balanceRepository.SaveChanges();

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

                _context.SaveChanges();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Reject Leave
        public void Reject(int requestId, string rejectionReason)
        {
            var request = _requestRepository.GetById(requestId);

            if (request == null)
                throw new ArgumentException("Leave request not found.");

            if (request.Status != "Pending")
                throw new ArgumentException("Only pending requests can be rejected.");

            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new ArgumentException("Rejection reason is required.");

            if (rejectionReason.Length > 500)
                throw new ArgumentException("Rejection reason cannot exceed 500 characters.");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                string oldValues = $"Status: {request.Status}";

                request.Status = "Rejected";
                request.RejectionReason = rejectionReason.Trim();

                _requestRepository.Update(request);

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

                _context.SaveChanges();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
