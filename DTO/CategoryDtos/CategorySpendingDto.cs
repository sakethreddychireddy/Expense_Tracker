namespace Expense_Tracker.DTO.CategoryDtos
{
    public class CategorySpendingDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } =string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
