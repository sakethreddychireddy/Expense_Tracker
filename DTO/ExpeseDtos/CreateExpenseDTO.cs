using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.DTO.ExpeseDtos
{
    public class CreateExpenseDTO
    {
        public string Title { get; set; } = string.Empty;

        
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        
        public int CategoryId { get; set; } 
    }
}
