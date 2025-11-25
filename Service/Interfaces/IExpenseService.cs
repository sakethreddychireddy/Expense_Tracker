using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.DTO.PaginationDtos;
namespace Expense_Tracker.Service.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseDto?> CreateExpenseAsync(CreateExpenseDTO createExpenseDto,int userId);
        Task<PagedResult<ExpenseDto>> GetAllExpenses(int userId, PaginationParams paginationParams);

        Task<UpdateExpenseDto?> UpdateExpenseAsync(int id, UpdateExpenseDto updateExpenseDto,int userId);
        Task<bool> DeleteExpenseAsync(int id,int userId);
        Task<ExpenseDto> GetExpenseByIdAsync(int id);
        Task<decimal> GetTotalExpensesAsync(int userId);
        Task <IEnumerable<MonthlyExpnseDto>> GetMonthlyExpnseAsync(int userId);
        Task<List<CategorySpendingDto>> GetSpendingByCategoryAsync(int userId);
    }
}
