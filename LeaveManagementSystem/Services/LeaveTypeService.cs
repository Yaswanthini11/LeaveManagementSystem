
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

        // Get all leave types
        public async Task<List<LeaveType>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Add Leave Type
        public async Task AddAsync(LeaveType leaveType)
        {
            Validate(leaveType);

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                await _repository.AddAsync(leaveType);

                _auditLogService.Log(
                    "Admin",
                    "Created",
                    "LeaveType",
                    leaveType.LeaveTypeId,
                    "New leave type created",
                    null,
                    $"Name: {leaveType.Name}, Default Days: {leaveType.DefaultDays}"
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

        // Update Leave Type
        public async Task<bool> UpdateAsync(LeaveType leaveType)
        {
            Validate(leaveType);

            if (leaveType.LeaveTypeId <= 0)
                throw new ArgumentException("Invalid leave type ID.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var oldLeaveType =
                    await _repository.GetByIdAsync(leaveType.LeaveTypeId);

                if (oldLeaveType == null)
                    return false;

                string oldValues =
                    $"Name: {oldLeaveType.Name}, Default Days: {oldLeaveType.DefaultDays}";

                bool updated = await _repository.UpdateAsync(leaveType);

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

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Delete Leave Type
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid leave type ID.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var leaveType = await _repository.GetByIdAsync(id);

                if (leaveType == null)
                    return false;

                string oldValues =
                    $"Name: {leaveType.Name}, Default Days: {leaveType.DefaultDays}";

                bool deleted = await _repository.DeleteAsync(id);

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

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
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
