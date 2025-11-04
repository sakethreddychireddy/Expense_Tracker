namespace Expense_Tracker.DTO.RecurringDtos
{
    public class RecurringExpenseDto
    {
        public int Id { get; set; }          // ✅ Unique expense Id (needed for edit/delete)
        public int UserId { get; set; }      // User who owns the expense
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
