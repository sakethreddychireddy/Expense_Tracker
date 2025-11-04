namespace Expense_Tracker.DTO.CategoryDtos
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
