using Microsoft.AspNetCore.Mvc;
using NineCafeApi.Authorization;
using NineCafeApi.DTOs.Debts;
using NineCafeApi.Services.Debts;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Perm.DEBT_READ)]
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
            var debts = await _debtService.GetAllDebtsAsync();
            return Ok(debts);
        }

        // GET: api/debts/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DebtResponseDto>> GetDebtById(int id)
        {
            var debt = await _debtService.GetDebtByIdAsync(id);
            if (debt == null)
            {
                return NotFound(new { message = $"Debt with ID {id} was not found." });
            }
            return Ok(debt);
        }

        // POST: api/debts
        [HttpPost]
        [HasPermission(Perm.DEBT_CREATE)]
        public async Task<ActionResult<DebtResponseDto>> CreateDebt([FromBody] DebtCreateDto dto)
        {
            var createdDebt = await _debtService.CreateDebtAsync(dto);
            return CreatedAtAction(nameof(GetDebtById), new { id = createdDebt.Id }, createdDebt);
        }

        // PUT: api/debts/5
        [HttpPut("{id:int}")]
        [HasPermission(Perm.DEBT_UPDATE)]
        public async Task<ActionResult<DebtResponseDto>> UpdateDebt(int id, [FromBody] DebtCreateDto dto)
        {
            var updatedDebt = await _debtService.UpdateDebtAsync(id, dto);
            if (updatedDebt == null)
            {
                return NotFound(new { message = $"Debt with ID {id} was not found." });
            }
            return Ok(updatedDebt);
        }

        // PATCH: api/debts/5/pay
        [HttpPatch("{id:int}/pay")]
        [HasPermission(Perm.DEBT_UPDATE)]
        public async Task<ActionResult<DebtResponseDto>> MarkDebtAsPaid(int id)
        {
            var updatedDebt = await _debtService.MarkDebtAsPaidAsync(id);
            if (updatedDebt == null)
            {
                return NotFound(new { message = $"Debt with ID {id} was not found." });
            }
            return Ok(updatedDebt);
        }

        // DELETE: api/debts/5
        [HttpDelete("{id:int}")]
        [HasPermission(Perm.DEBT_DELETE)]
        public async Task<IActionResult> DeleteDebt(int id)
        {
            var success = await _debtService.DeleteDebtAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Debt with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
