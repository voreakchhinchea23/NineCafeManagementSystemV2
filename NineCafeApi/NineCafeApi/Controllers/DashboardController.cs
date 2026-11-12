using Microsoft.AspNetCore.Mvc;
using NineCafeApi.Authorization;
using NineCafeApi.DTOs.Dashboard;
using NineCafeApi.DTOs.Withdrawals;
using NineCafeApi.Services.Dashboard;
using NineCafeApi.Services.Withdrawals;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Perm.DASHBOARD_READ)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IWithdrawalService _withdrawalService;

        public DashboardController(IDashboardService dashboardService, IWithdrawalService withdrawalService)
        {
            _dashboardService = dashboardService;
            _withdrawalService = withdrawalService;
        }

        // ==========================================
        // 1. DASHBOARD OVERVIEW
        // ==========================================

        // GET: api/dashboard?date=2026-11-07
        [HttpGet]
        public async Task<ActionResult<DashboardResponseDto>> GetDashboard([FromQuery] DateOnly? date = null)
        {
            var selectedDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var dashboard = await _dashboardService.GetDashboardByDateAsync(selectedDate);
            return Ok(dashboard);
        }

        // ==========================================
        // 2. DAILY SALES
        // ==========================================

        // GET: api/dashboard/sales?date=2026-11-07
        [HttpGet("sales")]
        public async Task<ActionResult<IEnumerable<DailySaleResponseDto>>> GetSales([FromQuery] DateOnly? date = null)
        {
            var sales = await _dashboardService.GetAllSalesAsync(date);
            return Ok(sales);
        }

        // GET: api/dashboard/sales/5
        [HttpGet("sales/{id:int}")]
        public async Task<ActionResult<DailySaleResponseDto>> GetSaleById(int id)
        {
            var sale = await _dashboardService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                return NotFound(new { message = $"DailySale with ID {id} was not found." });
            }
            return Ok(sale);
        }

        // POST: api/dashboard/sales
        [HttpPost("sales")]
        [HasPermission(Perm.DASHBOARD_CREATE)]
        public async Task<ActionResult<DailySaleResponseDto>> RecordSale([FromBody] DailySaleCreateDto dto)
        {
            try
            {
                var sale = await _dashboardService.RecordSaleAsync(dto);
                return CreatedAtAction(nameof(GetSaleById), new { id = sale.Id }, sale);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/dashboard/sales/5
        [HttpPut("sales/{id:int}")]
        [HasPermission(Perm.DASHBOARD_UPDATE)]
        public async Task<ActionResult<DailySaleResponseDto>> UpdateSaleQuantity(int id, [FromBody] DailySaleUpdateQuantityDto dto)
        {
            if (dto.Quantity == 0)
            {
                var deleted = await _dashboardService.DeleteSaleAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"DailySale with ID {id} was not found." });
                }
                return NoContent();
            }

            var updatedSale = await _dashboardService.UpdateSaleQuantityAsync(id, dto.Quantity);
            if (updatedSale == null)
            {
                return NotFound(new { message = $"DailySale with ID {id} was not found." });
            }
            return Ok(updatedSale);
        }

        // PUT: api/dashboard/sales/price/1?date=2026-11-07
        [HttpPut("sales/price/{priceId:int}")]
        [HasPermission(Perm.DASHBOARD_UPDATE)]
        public async Task<ActionResult<DailySaleResponseDto>> SetSaleQuantityByPrice(
            int priceId,
            [FromBody] DailySaleUpdateQuantityDto dto,
            [FromQuery] DateOnly? date = null)
        {
            try
            {
                var result = await _dashboardService.SetSaleQuantityByPriceAsync(priceId, dto.Quantity, date);
                if (result == null)
                {
                    return NoContent();
                }
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/dashboard/sales/5
        [HttpDelete("sales/{id:int}")]
        [HasPermission(Perm.DASHBOARD_DELETE)]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var success = await _dashboardService.DeleteSaleAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"DailySale with ID {id} was not found." });
            }
            return NoContent();
        }

        // ==========================================
        // 3. WITHDRAWALS
        // ==========================================

        // GET: api/dashboard/withdrawals?date=2026-11-07
        [HttpGet("withdrawals")]
        public async Task<ActionResult<IEnumerable<WithdrawalResponseDto>>> GetWithdrawals([FromQuery] DateOnly? date = null)
        {
            var withdrawals = await _withdrawalService.GetAllWithdrawalsAsync(date);
            return Ok(withdrawals);
        }

        // GET: api/dashboard/withdrawals/5
        [HttpGet("withdrawals/{id:int}")]
        public async Task<ActionResult<WithdrawalResponseDto>> GetWithdrawalById(int id)
        {
            var withdrawal = await _withdrawalService.GetWithdrawalByIdAsync(id);
            if (withdrawal == null)
            {
                return NotFound(new { message = $"Withdrawal with ID {id} was not found." });
            }
            return Ok(withdrawal);
        }

        // POST: api/dashboard/withdrawals
        [HttpPost("withdrawals")]
        [HasPermission(Perm.DASHBOARD_CREATE)]
        public async Task<ActionResult<WithdrawalResponseDto>> RecordWithdrawal([FromBody] WithdrawalCreateDto dto)
        {
            var createdWithdrawal = await _withdrawalService.CreateWithdrawalAsync(dto);
            return CreatedAtAction(nameof(GetWithdrawalById), new { id = createdWithdrawal.Id }, createdWithdrawal);
        }

        // PUT: api/dashboard/withdrawals/5
        [HttpPut("withdrawals/{id:int}")]
        [HasPermission(Perm.DASHBOARD_UPDATE)]
        public async Task<ActionResult<WithdrawalResponseDto>> UpdateWithdrawal(int id, [FromBody] WithdrawalCreateDto dto)
        {
            var updatedWithdrawal = await _withdrawalService.UpdateWithdrawalAsync(id, dto);
            if (updatedWithdrawal == null)
            {
                return NotFound(new { message = $"Withdrawal with ID {id} was not found." });
            }
            return Ok(updatedWithdrawal);
        }

        // DELETE: api/dashboard/withdrawals/5
        [HttpDelete("withdrawals/{id:int}")]
        [HasPermission(Perm.DASHBOARD_DELETE)]
        public async Task<IActionResult> DeleteWithdrawal(int id)
        {
            var success = await _withdrawalService.DeleteWithdrawalAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Withdrawal with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
