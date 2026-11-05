using Microsoft.AspNetCore.Mvc;
using NineCafeApi.DTOs.Debts;
using NineCafeApi.Services.Debts;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebtsController : ControllerBase
    {
        private readonly IDebtService _debtService;

        public DebtsController(IDebtService debtService)
        {
            _debtService = debtService;
        }

        // GET: api/debts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DebtResponseDto>>> GetDebts()
        {
            var debts = await _debtService.GetAllAsync();
            return Ok(debts);
        }

        // GET: api/debts/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DebtResponseDto>> GetDebtById(int id)
        {
            var debt = await _debtService.GetByIdAsync(id);
            if (debt == null)
            {
                return NotFound(new { message = $"Debt with ID {id} was not found." });
            }
            return Ok(debt);
        }

        // POST: api/debts
        [HttpPost]
        public async Task<ActionResult<DebtResponseDto>> CreateDebt([FromBody] DebtCreateDto dto)
        {
            var createdDebt = await _debtService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDebtById), new { id = createdDebt.Id }, createdDebt);
        }

        // PUT: api/debts/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<DebtResponseDto>> UpdateDebt(int id, [FromBody] DebtCreateDto dto)
        {
            var updatedDebt = await _debtService.UpdateAsync(id, dto);
            if (updatedDebt == null)
            {
                return NotFound(new { message = $"Debt with ID {id} was not found." });
            }
            return Ok(updatedDebt);
        }

        // PATCH: api/debts/5/pay
        [HttpPatch("{id:int}/pay")]
        public async Task<ActionResult<DebtResponseDto>> MarkDebtAsPaid(int id)
        {
            var updatedDebt = await _debtService.MarkAsPaidAsync(id);
            if (updatedDebt == null)
            {
                return NotFound(new { message = $"Debt with ID {id} was not found." });
            }
            return Ok(updatedDebt);
        }

        // DELETE: api/debts/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDebt(int id)
        {
            var success = await _debtService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Debt with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
