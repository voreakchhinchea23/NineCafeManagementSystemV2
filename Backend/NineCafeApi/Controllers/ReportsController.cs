using Microsoft.AspNetCore.Mvc;
using NineCafeApi.Authorization;
using NineCafeApi.DTOs.Reports;
using NineCafeApi.Services.Reports;

namespace NineCafeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [HasPermission(Perm.REPORT_READ)]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: api/reports/monthly?year=2026&month=11
        [HttpGet("monthly")]
        public async Task<ActionResult<MonthlyReportResponseDto>> GetMonthlyReport(
            [FromQuery] int? year = null,
            [FromQuery] int? month = null)
        {
            var now = DateTime.Today;
            var reportYear = year ?? now.Year;
            var reportMonth = month ?? now.Month;

            if (reportMonth < 1 || reportMonth > 12)
            {
                return BadRequest(new { message = "Month must be between 1 and 12." });
            }

            if (reportYear < 2000 || reportYear > 2100)
            {
                return BadRequest(new { message = "Invalid year." });
            }

            var report = await _reportService.GetMonthlyReportAsync(reportYear, reportMonth);
            return Ok(report);
        }
    }
}
