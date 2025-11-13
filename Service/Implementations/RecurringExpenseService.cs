using Expense_Tracker.Data;
using Expense_Tracker.DTO.RecurringDtos;
using Expense_Tracker.Models;
using Expense_Tracker.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Service.Implementations
{
    public class RecurringExpenseService : IRecurringExpenseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RecurringExpenseService> _logger;

        public RecurringExpenseService(AppDbContext context, ILogger<RecurringExpenseService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<RecurringExpenseDto?> AddRecurringExpenseAsync(int userId, CreateRecurringExpenseDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return null;
                }
                var recurringExpense = new RecurringExpense
                {
                    UserId = userId,
                    Title = dto.Title,
                    Amount = dto.Amount,
                    Category = dto.Category,
                    Frequency = dto.Frequency,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    NextOccurrence = dto.StartDate,
                    IsActive = true

                };
                await _context.RecurringExpenses.AddAsync(recurringExpense);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Recurring Expense with ID {ExpenseId} created successfully for User {UserId}.", recurringExpense.Id, userId);
                return new RecurringExpenseDto
                {
                    Id = recurringExpense.Id,               
                    UserId = recurringExpense.UserId,       
                    Title = recurringExpense.Title,
                    Amount = recurringExpense.Amount,
                    Category = recurringExpense.Category,
                    Frequency = recurringExpense.Frequency,
                    StartDate = recurringExpense.StartDate,
                    EndDate = recurringExpense.EndDate,
                    IsActive = recurringExpense.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding recurring expense for User {UserId}.", userId);
                throw;
            }
        }
        public async Task<IEnumerable<RecurringExpenseDto>> GetUserRecurringExpensesAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return Enumerable.Empty<RecurringExpenseDto>();
                }
                var recurringExpenses = await _context.RecurringExpenses
                    .Where(re => re.UserId == userId && re.IsActive).Select(re => new RecurringExpenseDto
                    {
                        Id = re.Id,
                        UserId = re.UserId,
                        Title = re.Title,
                        Amount = re.Amount,
                        Category = re.Category,
                        Frequency = re.Frequency,
                        StartDate = re.StartDate,
                        EndDate = re.EndDate,
                        IsActive = re.IsActive
                    }).ToListAsync();
                _logger.LogInformation("Retrieved {Count} recurring expenses for User {UserId}.", recurringExpenses.Count, userId);
                return recurringExpenses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recurring expenses for User {UserId}.", userId);
                throw;
            }
        }
        public async Task<RecurringExpenseDto?> UpdateRecurringExpenseAsync(int id, int userId, UpdateRecurringExpenseDto dto)
        {
            try
            {
                var recurringExpense = await _context.RecurringExpenses.FirstOrDefaultAsync(re => re.Id == id && re.UserId == userId);
                if (recurringExpense == null)
                {
                    _logger.LogWarning("Recurring Expense with ID {ExpenseId} for User {UserId} not found.", id, userId);
                    return null;
                }
                if (!recurringExpense.IsActive)
                {
                    _logger.LogWarning("Recurring Expense with ID {ExpenseId} for User {UserId} is inactive and cannot be updated.", id, userId);
                    return null;
                }
                recurringExpense.Title = dto.Title;
                recurringExpense.Amount = dto.Amount;
                recurringExpense.Category = dto.Category;
                recurringExpense.Frequency = dto.Frequency;
                recurringExpense.StartDate = dto.StartDate;
                recurringExpense.EndDate = dto.EndDate;

                _context.RecurringExpenses.Update(recurringExpense);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Recurring Expense with ID {ExpenseId} for User {UserId} updated successfully.", id, userId);
                return new RecurringExpenseDto
                {
                    Id = recurringExpense.Id,               
                    UserId = recurringExpense.UserId,       
                    Title = recurringExpense.Title,
                    Amount = recurringExpense.Amount,
                    Category = recurringExpense.Category,
                    Frequency = recurringExpense.Frequency,
                    StartDate = recurringExpense.StartDate,
                    EndDate = recurringExpense.EndDate,
                    IsActive = recurringExpense.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Recurring Expense with ID {ExpenseId} for User {UserId}.", id, userId);
                throw;
            }
        }
        public async Task<bool> DeleteRecurringExpenseAsync(int id, int userId)
        {
            try
            {
                var recurringExpense = await _context.RecurringExpenses.FirstOrDefaultAsync(re => re.Id == id && re.UserId == userId);
                if (recurringExpense == null)
                {
                    _logger.LogWarning("Recurring Expense with ID {ExpenseId} for User {UserId} not found.", id, userId);
                    return false;
                }
                if (recurringExpense.IsActive == false)
                {
                    _logger.LogWarning("Recurring Expense with ID {ExpenseId} for User {UserId} is already inactive.", id, userId);
                    return false;
                }
                recurringExpense.IsActive = false;
                _context.RecurringExpenses.Remove(recurringExpense);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Recurring Expense with ID {ExpenseId} for User {UserId} marked as inactive.", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Recurring Expense with ID {ExpenseId} for User {UserId}.", id, userId);
                throw;
            }
        }
        public async Task<RecurringExpenseDto?> GetRecurringExpenseByIdAsync(int id)
        {
            try
            {
                var recurringExpense = await _context.RecurringExpenses.FindAsync(id);
                if (recurringExpense == null)
                {
                    _logger.LogWarning("Recurring Expense with ID {ExpenseId} not found.", id);
                    return null;
                }
                return new RecurringExpenseDto
                {
                    Id = recurringExpense.Id,             
                    UserId = recurringExpense.UserId,      
                    Title = recurringExpense.Title,
                    Amount = recurringExpense.Amount,
                    Category = recurringExpense.Category,
                    Frequency = recurringExpense.Frequency,
                    StartDate = recurringExpense.StartDate,
                    EndDate = recurringExpense.EndDate,
                    IsActive = recurringExpense.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Recurring Expense with ID {ExpenseId}.", id);
                throw;
            }
        }
    }
}
