using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.DTO.ExpeseDtos;
namespace Expense_Tracker.Service.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseDto?> CreateExpenseAsync(CreateExpenseDTO createExpenseDto,int userId);
        Task<IEnumerable<ExpenseDto>> GetAllExpenses(int userId);
        Task <ExpenseDto?> UpdateExpenseAsync(int id, UpdateExpenseDto updateExpenseDto,int userId);
        Task<bool> DeleteExpenseAsync(int id,int userId);
        Task<ExpenseDto> GetExpenseByIdAsync(int id);
        Task<decimal> GetTotalExpensesAsync(int userId);
        Task <List<MonthlyExpnseDto>> GetMonthlyExpenseAsync(int userId);
        Task<List<CategorySpendingDto>> GetSpendingByCategoryAsync(int userId);
    }
}
