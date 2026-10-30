
using LeaveManagementSystem.Data;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Services
{
    public class LeaveTypeService
    {
        private readonly LeaveTypeRepository _repository;
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;

        public LeaveTypeService(
            LeaveTypeRepository repository,
            AppDbContext context,
            AuditLogService auditLogService)
        {
            _repository = repository;
            _context = context;
            _auditLogService = auditLogService;
        }

        public List<LeaveType> GetAll()
        {
            return _repository.GetAll();
        }

        // Add Leave Type
        public void Add(LeaveType leaveType)
        {
            Validate(leaveType);

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _repository.Add(leaveType);

                _auditLogService.Log(
                    "Admin",
                    "Created",
                    "LeaveType",
                    leaveType.LeaveTypeId,
                    "New leave type created",
                    null,
                    $"Name: {leaveType.Name}, Default Days: {leaveType.DefaultDays}"
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

        // Update Leave Type
        public bool Update(LeaveType leaveType)
        {
            Validate(leaveType);

            if (leaveType.LeaveTypeId <= 0)
                throw new ArgumentException("Invalid leave type ID.");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var oldLeaveType = _repository.GetById(leaveType.LeaveTypeId);

                if (oldLeaveType == null)
                    return false;

                string oldValues =
                    $"Name: {oldLeaveType.Name}, Default Days: {oldLeaveType.DefaultDays}";

                bool updated = _repository.Update(leaveType);

                if (!updated)
                    return false;

                string newValues =
                    $"Name: {leaveType.Name}, Default Days: {leaveType.DefaultDays}";

                _auditLogService.Log(
                    "Admin",
                    "Updated",
                    "LeaveType",
                    leaveType.LeaveTypeId,
                    "Leave type details updated",
                    oldValues,
                    newValues
                );

                _context.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Delete Leave Type
        public bool Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid leave type ID.");

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var leaveType = _repository.GetById(id);

                if (leaveType == null)
                    return false;

                string oldValues =
                    $"Name: {leaveType.Name}, Default Days: {leaveType.DefaultDays}";

                bool deleted = _repository.Delete(id);

                if (!deleted)
                    return false;

                _auditLogService.Log(
                    "Admin",
                    "Deleted",
                    "LeaveType",
                    id,
                    "Leave type deleted",
                    oldValues,
                    null
                );

                _context.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static void Validate(LeaveType leaveType)
        {
            if (leaveType == null)
                throw new ArgumentNullException(nameof(leaveType));

            if (string.IsNullOrWhiteSpace(leaveType.Name))
                throw new ArgumentException("Leave type name is required.");

            if (leaveType.DefaultDays <= 0)
                throw new ArgumentException("Default days must be greater than zero.");
        }
    }
}
