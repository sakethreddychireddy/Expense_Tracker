using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.Models;

namespace Expense_Tracker.Repositories.Interfaces
{
    public interface IExpenseRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<Expense?> CreateExpenseAsync(Expense expense,int userId);
        Task<IEnumerable<Expense?>> GetAllExpensesAsync(int userId);
        Task<Expense?> GetExpenseByIdAsync(int id);
        Task<Expense?> UpdateExpenseAsync(int id, Expense expense, int userId);
        Task<bool> DeleteExpenseAsync(int Id, int userId);
        Task<decimal> GetTotalExpensesAsync(int userId);
        Task<IEnumerable<MonthlyExpnseDto>> GetMonthlyExpensesAsync(int userId);
        Task<List<CategorySpendingDto>> GetSpendingByCategorisAsync(int userId);

    }
}
