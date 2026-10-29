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

        public LeaveRequestService(
            LeaveRequestRepository requestRepository,
            EmployeeRepository employeeRepository,
            LeaveTypeRepository leaveTypeRepository,
            LeaveBalanceRepository balanceRepository)
        {
            _requestRepository = requestRepository;
            _employeeRepository = employeeRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _balanceRepository = balanceRepository;
        }

        public List<LeaveRequest> GetAll()
        {
            return _requestRepository.GetAll();
        }

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

            _requestRepository.Add(new LeaveRequest
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                TotalDays = totalDays,
                Reason = reason.Trim(),
                Status = "Pending",
                AppliedOn = DateTime.Now
            });
        }

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

            balance.UsedDays += request.TotalDays;

            request.Status = "Approved";
            _requestRepository.Update(request);
            _balanceRepository.SaveChanges();
        }

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

            request.Status = "Rejected";
            request.RejectionReason = rejectionReason.Trim();
            _requestRepository.Update(request);
        }
    }
}