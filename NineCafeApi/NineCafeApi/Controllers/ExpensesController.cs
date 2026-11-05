using Microsoft.AspNetCore.Mvc;
using NineCafeApi.DTOs.Expenses;
using NineCafeApi.Services.Expenses;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var expenses = await _expenseService.GetAllAsync();
            return Ok(expenses);
        }

        // GET: api/expenses/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExpenseResponseDto>> GetExpenseById(int id)
        {
            var expense = await _expenseService.GetByIdAsync(id);
            if (expense == null)
            {
                return NotFound(new { message = $"Expense with ID {id} was not found." });
            }
            return Ok(expense);
        }

        // POST: api/expenses
        [HttpPost]
        public async Task<ActionResult<ExpenseResponseDto>> CreateExpense([FromBody] ExpenseCreateDto dto)
        {
            try
            {
                var createdExpense = await _expenseService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetExpenseById), new { id = createdExpense.Id }, createdExpense);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/expenses/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExpenseResponseDto>> UpdateExpense(int id, [FromBody] ExpenseCreateDto dto)
        {
            try
            {
                var updatedExpense = await _expenseService.UpdateAsync(id, dto);
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
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var success = await _expenseService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Expense with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
