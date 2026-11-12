using Microsoft.AspNetCore.Mvc;
using NineCafeApi.Authorization;
using NineCafeApi.DTOs.Expenses;
using NineCafeApi.Services.Expenses;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Perm.EXPENSE_READ)]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // GET: api/expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseResponseDto>>> GetExpenses()
        {
            var expenses = await _expenseService.GetAllExpensesAsync();
            return Ok(expenses);
        }

        // GET: api/expenses/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExpenseResponseDto>> GetExpenseById(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound(new { message = $"Expense with ID {id} was not found." });
            }
            return Ok(expense);
        }

        // POST: api/expenses
        [HttpPost]
        [HasPermission(Perm.EXPENSE_CREATE)]
        public async Task<ActionResult<ExpenseResponseDto>> CreateExpense([FromBody] ExpenseCreateDto dto)
        {
            try
            {
                var createdExpense = await _expenseService.CreateExpenseAsync(dto);
                return CreatedAtAction(nameof(GetExpenseById), new { id = createdExpense.Id }, createdExpense);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/expenses/5
        [HttpPut("{id:int}")]
        [HasPermission(Perm.EXPENSE_UPDATE)]
        public async Task<ActionResult<ExpenseResponseDto>> UpdateExpense(int id, [FromBody] ExpenseCreateDto dto)
        {
            try
            {
                var updatedExpense = await _expenseService.UpdateExpenseAsync(id, dto);
                if (updatedExpense == null)
                {
                    return NotFound(new { message = $"Expense with ID {id} was not found." });
                }
                return Ok(updatedExpense);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/expenses/5
        [HttpDelete("{id:int}")]
        [HasPermission(Perm.EXPENSE_DELETE)]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var success = await _expenseService.DeleteExpenseAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Expense with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
