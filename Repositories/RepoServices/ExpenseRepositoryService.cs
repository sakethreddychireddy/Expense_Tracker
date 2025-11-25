using Expense_Tracker.Data;
using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.Models;
using Expense_Tracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expense_Tracker.Repositories.RepoServices
{
    public class ExpenseRepositoryService : IExpenseRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExpenseRepositoryService> _logger;
        public ExpenseRepositoryService(AppDbContext context, ILogger<ExpenseRepositoryService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        public async Task<Expense?> CreateExpenseAsync(Expense expense)
        {
            try
            {
                _logger.LogInformation("Creating expense for User ID");
                var result = await _context.Expenses.AddAsync(expense);
                _logger.LogInformation("Expense created with ID {ExpenseId} for User ID.", result.Entity.Id);
                await _context.SaveChangesAsync();
                return expense;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense");
                return null;
            }
        }
        public async Task<IEnumerable<Expense?>> GetAllExpensesAsync(int userId, int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Retrieving all expenses for User ID {UserId}.", userId);
                var getAllExpenses = await _context.Expenses
                    .Where(e => e.UserId == userId)
                    .Include(c => c.Category)
                    .OrderByDescending(e => e.Date)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} expenses for User ID {UserId}.", getAllExpenses.Count, userId);
                return getAllExpenses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expenses");
                throw;
            }
        }
        public async Task<int> GetExpenseCountAsync(int userId)
        {
            _logger.LogInformation("Counting expenses for User ID {UserId}.", userId);
            return await _context.Expenses.CountAsync(e => e.UserId == userId);
        }
        public async Task<Expense?> GetExpenseByIdAsync(int expenseId, int userId)
        {
            _logger.LogInformation("Retrieving expense with ID {ExpenseId} for User ID {UserId}.", expenseId, userId);
            return await _context.Expenses
                .Include(c => c.Category)
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId);
        }
        public async Task<Expense?> UpdateExpenseAsync(int id, Expense expense)
        {
            try
            {
             _logger.LogInformation("Updating expense with ID {ExpenseId}", id);
                var updateExpense = await _context.Expenses.Include(c => c.Category)
                    .FirstOrDefaultAsync(e => e.Id == id);
                if (updateExpense == null)
                    return null;
                // Update fields
                _logger.LogInformation("Found expense with ID {ExpenseId}. Updating fields.", id);
                updateExpense.Title = expense.Title;
                updateExpense.Amount = expense.Amount;
                updateExpense.Date = expense.Date;
                updateExpense.CategoryId = expense.CategoryId;
                _logger.LogInformation("Saving updated expense with ID {ExpenseId}.", id);
                _context.Expenses.Update(updateExpense);
                await _context.SaveChangesAsync();
                await _context.Entry(updateExpense).Reference(e => e.Category).LoadAsync();
                return updateExpense;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense");
                return null;
            }

        }
        public async Task<Expense?> GetExpenseByIdAsync(int id)
        {
            try
            {
                var getExpenseById = await _context.Expenses.Include(c => c.Category)
                    .FirstOrDefaultAsync(e => e.Id == id);
                return getExpenseById;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error while fetching the expense");
                return null;
            }
        }
        public async Task<bool> DeleteExpenseAsync(int id, int userId)
        {
            try
            {
                _logger.LogInformation("Deleting expense with ID {ExpenseId} for User ID {UserId}.", id, userId);
                var deleteExpense = await _context.Expenses
                    .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
                if (deleteExpense == null)
                    return false;
                _logger.LogInformation("Found expense with ID {ExpenseId}. Deleting expense.", id);
                _context.Expenses.Remove(deleteExpense);
                _logger.LogInformation("Expense with ID {ExpenseId} deleted successfully.", id);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense");
                return false;
            }
        }
        public async Task<decimal> GetTotalExpensesAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Calculating total expenses for User ID {UserId}.", userId);
                var total = await _context.Expenses
                    .Where(e => e.UserId == userId)
                    .SumAsync(e => e.Amount);
                _logger.LogInformation("Total expenses for User ID {UserId} is {Total}.", userId, total);
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total expenses");
                return 0;
            }
        }
        public async Task<IEnumerable<MonthlyExpnseDto>> GetMonthlyExpensesAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Calculating monthly expenses for User ID {UserId}.", userId);
                var allMonths = Enumerable.Range(1, 12)
                    .Select(m => new MonthlyExpnseDto
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m),
                        TotalAmount = 0
                    }).ToList();
                _logger.LogInformation("Initialized all months with zero expenses for User ID {UserId}.", userId);
                var expenses = await _context.Expenses
                    .Where(e => e.UserId == userId)
                    .GroupBy(e => e.Date.Month)
                    .Select(g => new MonthlyExpnseDto
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        TotalAmount = g.Sum(e => e.Amount)
                    }).AsNoTracking()
                    .ToListAsync();
                _logger.LogInformation("Grouped expenses by month for User ID {UserId}.", userId);
                // Merge so months with no expenses show 0
                foreach (var exp in expenses)
                {
                    var month = allMonths.First(m => m.Month == exp.Month);
                    month.TotalAmount = exp.TotalAmount;
                }
                _logger.LogInformation("Retrieved {Count} monthly expenses for User ID {UserId}.", allMonths.Count, userId);
                return allMonths;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly expenses");
                throw;
            }
        }
        public async Task<List<CategorySpendingDto>> GetSpendingByCategorisAsync(int userId)
        {
            try
            {
                _logger.LogInformation("fetching the spedings by category");
                var spendingByCategory = await _context.Database
                    .SqlQuery<CategorySpendingDto>($"EXEC getCategorySpendingsByUser @UserId = {userId}")
                    .AsNoTracking()
                    .ToListAsync();
                return spendingByCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retriveing spendings by category");
                throw;
            }

        }
    }
}
