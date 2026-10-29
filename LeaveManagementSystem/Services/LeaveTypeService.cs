using LeaveManagementSystem.Models;
using LeaveManagementSystem.Repositories;

namespace LeaveManagementSystem.Services
{
    public class LeaveTypeService
    {
        private readonly LeaveTypeRepository _repository;

        public LeaveTypeService(LeaveTypeRepository repository)
        {
            _repository = repository;
        }

        public List<LeaveType> GetAll()
        {
            return _repository.GetAll();
        }

        public void Add(LeaveType leaveType)
        {
            Validate(leaveType);
            _repository.Add(leaveType);
        }

        public bool Update(LeaveType leaveType)
        {
            Validate(leaveType);
            return _repository.Update(leaveType);
        }

        public bool Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid leave type ID.");

            return _repository.Delete(id);
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