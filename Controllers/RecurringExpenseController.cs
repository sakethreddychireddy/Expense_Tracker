using Expense_Tracker.Data;
using Expense_Tracker.DTO;
using Expense_Tracker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Expense_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringExpenseController : ControllerBase
    {
        private readonly IRecurringExpenseService _recurringExpenseService;
        private readonly AppDbContext appDbContext;
        private readonly ILogger<RecurringExpenseController> _logger;

        public RecurringExpenseController(IRecurringExpenseService recurringExpenseService, AppDbContext appDbContext, ILogger<RecurringExpenseController> logger)
        {
            _recurringExpenseService = recurringExpenseService;
            this.appDbContext = appDbContext;
            _logger = logger;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token.");

            return int.Parse(userIdClaim);
        }
        [Authorize(Roles = "User")]
        [HttpPost("AddRecurringExpense")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddRecurringExpense([FromBody] CreateRecurringExpenseDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Invalid recurring expense data.");
                }
                int userId = GetUserId();
                _logger.LogInformation("Resolved UserId: {UserId}", userId);
                var recurringExpense = await _recurringExpenseService.AddRecurringExpenseAsync(userId, dto);
                if (recurringExpense == null)
                {
                    return NotFound("User not found.");
                }
                return CreatedAtAction(nameof(AddRecurringExpense), new { id = recurringExpense.Id }, recurringExpense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding recurring expense.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetUserRecurringExpenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserRecurringExpenses()
        {
            try
            {
                int userId = GetUserId();
                var recurringExpenses = await _recurringExpenseService.GetUserRecurringExpensesAsync(userId);
                return Ok(recurringExpenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recurring expenses.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [Authorize(Roles = "User")]
        [HttpPut("UpdateRecurringExpense/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRecurringExpense(int id, [FromBody] UpdateRecurringExpenseDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Invalid recurring expense data.");
                }
                int userId = GetUserId();
                var updatedExpense = await _recurringExpenseService.UpdateRecurringExpenseAsync(id, userId, dto);
                if (updatedExpense == null)
                {
                    return NotFound("Recurring expense not found.");
                }
                return Ok(updatedExpense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recurring expense.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [Authorize(Roles = "User")]
        [HttpDelete("DeleteRecurringExpense/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRecurringExpense(int id)
        {
            try
            {
                int userId = GetUserId();
                var success = await _recurringExpenseService.DeleteRecurringExpenseAsync(id, userId);
                if (!success)
                {
                    return NotFound("Recurring expense not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recurring expense.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [Authorize(Roles = "User")]
        [HttpGet("GetRecurringExpenseById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecurringExpenseById(int id)
        {
            try
            {
                int userId = GetUserId();
                var expense = await _recurringExpenseService.GetRecurringExpenseByIdAsync(id);
                if (expense == null || expense.UserId != userId)
                {
                    return NotFound("Recurring expense not found.");
                }
                return Ok(expense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recurring expense by ID.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
