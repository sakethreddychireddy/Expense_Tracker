namespace Expense_Tracker.DTO
{
    public class CreateRecurringExpenseDto
    {
        public string Title { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Category { get; set; } = string.Empty;

        // e.g., "Daily", "Weekly", "Monthly", "Yearly"
        public string Frequency { get; set; } = "Monthly";

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }
    }
}
