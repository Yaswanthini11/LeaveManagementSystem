
using System.Net.Mail;

namespace LeaveManagementSystem.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var address = new MailAddress(email);

                return address.Address.Equals(
                    email.Trim(),
                    StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool IsValidDateRange(
            DateTime startDate,
            DateTime endDate)
        {
            return endDate.Date >= startDate.Date;
        }
    }
}
