using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.Models;

namespace Expense_Tracker.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<Expense?> CreateExpenseAsync(Expense expense);
        Task<IEnumerable<Expense?>> GetAllExpensesAsync(int userId, int pageNumber, int pageSize);
        Task<int> GetExpenseCountAsync(int userId);
        Task<Expense?> GetExpenseByIdAsync(int id);
        Task<Expense?> UpdateExpenseAsync(int id, Expense expense);
        Task<bool> DeleteExpenseAsync(int Id, int userId);
        Task<decimal> GetTotalExpensesAsync(int userId);
        Task<IEnumerable<MonthlyExpnseDto>> GetMonthlyExpensesAsync(int userId);
        Task<List<CategorySpendingDto>> GetSpendingByCategorisAsync(int userId);

    }
}
