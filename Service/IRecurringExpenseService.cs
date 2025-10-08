using Expense_Tracker.DTO;

namespace Expense_Tracker.Service
{
    public interface IRecurringExpenseService
    {
        Task<RecurringExpenseDto?> AddRecurringExpenseAsync(int userId, CreateRecurringExpenseDto dto);
        Task<IEnumerable<RecurringExpenseDto>> GetUserRecurringExpensesAsync(int userId);
        Task<RecurringExpenseDto?> GetRecurringExpenseByIdAsync(int id);
        Task<RecurringExpenseDto?> UpdateRecurringExpenseAsync(int id, int userId, UpdateRecurringExpenseDto dto);
        Task<bool> DeleteRecurringExpenseAsync(int id,int userId);
        
    }
}
