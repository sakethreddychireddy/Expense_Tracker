using AutoMapper;
using Expense_Tracker.DTO.CategoryDtos;
using Expense_Tracker.DTO.ExpeseDtos;
using Expense_Tracker.DTO.PaginationDtos;
using Expense_Tracker.Models;
using Expense_Tracker.Repositories.Interfaces;
using Expense_Tracker.Service.Interfaces;

namespace Expense_Tracker.Service.Implementations
{
    public class ExpenseService : IExpenseService
    {
        private readonly ILogger<ExpenseService> _logger;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;
        public ExpenseService(ILogger<ExpenseService> logger, IExpenseRepository expenseRepository, IMapper mapper)
        {
            _logger = logger;
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }
        public async Task<ExpenseDto?> CreateExpenseAsync(CreateExpenseDTO createExpenseDto, int userId)
        {
            try
            {
                _logger.LogInformation("Creating expense for User {UserId}.", userId);
                var createExpense = _mapper.Map<Expense>(createExpenseDto);
                createExpense.UserId = userId;
                _logger.LogInformation("Calling repository to create expense.");
                var expense = await _expenseRepository.CreateExpenseAsync(createExpense);
                _logger.LogInformation("Expense with ID {ExpenseId} created successfully for User {UserId}.", expense.Id, userId);
                return new ExpenseDto
                {
                    Id = expense.Id,
                    Title = expense.Title,
                    Amount = expense.Amount,
                    Date = expense.Date
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense for User {UserId}.", userId);
                throw;
            }
        }
        public async Task<PagedResult<ExpenseDto>> GetAllExpenses(int userId, PaginationParams paginationParams)
        {
            try
            {
                _logger.LogInformation("Retrieving all expenses.");
                var totalCount = await _expenseRepository.GetExpenseCountAsync(userId);
                var getAllExpenses = await _expenseRepository.GetAllExpensesAsync(userId,
                    paginationParams.PageNumber,
                    paginationParams.PageSize);
                _logger.LogInformation("Expenses retrieved successfully.");
                var expenseDtos = getAllExpenses.Select(e => new ExpenseDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    Amount = e.Amount,
                    Date = e.Date,
                    CategoryName = e.Category.Name
                }).ToList();
                _logger.LogInformation("Mapped expenses to DTOs successfully.");
                return new PagedResult<ExpenseDto>
                {
                    Items = expenseDtos,
                    TotalCount = totalCount,
                    PageNumber = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving expenses: {ex.Message}", ex);
                throw new ApplicationException("Failed to retrieve expenses.", ex);
            }
        }
        public async Task<UpdateExpenseDto?> UpdateExpenseAsync(int id, UpdateExpenseDto updateExpenseDto, int userId)
        {
            _logger.LogInformation($"Update expense with ID {id} for User {userId}");
            var updateExpense = _mapper.Map<Expense>(updateExpenseDto);
            _logger.LogInformation($"Calling repository to update expense");
            var expense = await _expenseRepository.UpdateExpenseAsync(id, updateExpense);
            if (expense == null)
                return null;
            _logger.LogInformation($"Expense with ID {id} updated successfully");
            return new UpdateExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                CategoryId = expense.CategoryId
            };
            //updateExpense.Id = expense.UserId;
            //return _mapper.Map<UpdateExpenseDto>(expense);
        }

        public async Task<bool> DeleteExpenseAsync(int id, int userId)
        {
            _logger.LogInformation($"Delete expense");
            var deleteExpense= await _expenseRepository.DeleteExpenseAsync(id, userId);
            _logger.LogInformation($"Expense deleted successfully");
            return deleteExpense;
        }
        public async Task<ExpenseDto> GetExpenseByIdAsync(int id)
        { 
            _logger.LogInformation("Retrieving expense with ID {ExpenseId}.", id);
            var expense = await _expenseRepository.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                _logger.LogWarning("Expense with ID {ExpenseId} not found.", id);
                throw new KeyNotFoundException($"Expense with ID {id} not found.");
            }
            _logger.LogInformation("Expense with ID {ExpenseId} retrieved successfully.", id);
            return new ExpenseDto
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                CategoryName = expense.Category.Name,
                CategoryId = expense.Category.Id,
            };
        }
        public async Task<decimal> GetTotalExpensesAsync(int userId)
        {
            _logger.LogInformation("Calculating total expenses for User {UserId}.", userId);
            try
            {
                var total = await _expenseRepository.GetTotalExpensesAsync(userId);
                _logger.LogInformation("Total expenses for User {UserId} calculated successfully.", userId);
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total expenses for User {UserId}.", userId);
                throw new Exception($"Error calculating total expenses: {ex.Message}", ex);
            }
        }
        public async Task<IEnumerable<MonthlyExpnseDto?>> GetMonthlyExpnseAsync(int userId)
        {
            _logger.LogInformation("Retrieving monthly expenses for User {UserId}.", userId);
            try
            {
                var monthlyExpenses = await _expenseRepository.GetMonthlyExpensesAsync(userId);
                _logger.LogInformation("Monthly expenses for User {UserId} retrieved successfully.", userId);
                return monthlyExpenses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving monthly expenses for User {UserId}.", userId);
                throw new Exception($"Error retrieving monthly expenses: {ex.Message}", ex);
            }
        }
        public async Task<List<CategorySpendingDto>> GetSpendingByCategoryAsync(int userId)
        {
            try
            {
                var categorySpendings = await _expenseRepository.GetSpendingByCategorisAsync(userId);
                _logger.LogInformation("Spending by category for User {UserId} retrieved successfully.", userId);
                return categorySpendings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving spending by category for User {UserId}.", userId);
                throw new Exception($"Error retrieving spending by category: {ex.Message}", ex);
            }
        }
    }
}