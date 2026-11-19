using NineCafeApi.DTOs.Reports;

namespace NineCafeApi.Services.Reports
{
    public interface IReportService
    {
        Task<MonthlyReportResponseDto> GetMonthlyReportAsync(int year, int month);
    }
}
