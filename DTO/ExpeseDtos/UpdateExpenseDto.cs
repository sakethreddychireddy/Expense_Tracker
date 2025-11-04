namespace Expense_Tracker.DTO.ExpeseDtos
{
    public class UpdateExpenseDto
    {
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int CategoryId { get; set; }
    }
}
