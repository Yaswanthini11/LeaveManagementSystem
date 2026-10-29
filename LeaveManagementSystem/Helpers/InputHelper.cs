using System.Globalization;

namespace LeaveManagementSystem.Helpers
{
    public static class InputHelper
    {
        public static string ReadText(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();

                Console.WriteLine("Input cannot be empty.");
            }
        }

        public static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;

                Console.WriteLine("Please enter a valid number.");
            }
        }
    }
}
