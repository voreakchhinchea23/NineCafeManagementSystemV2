using Microsoft.EntityFrameworkCore;
using NineCafeApi.Data;
using NineCafeApi.DTOs.Reports;

namespace NineCafeApi.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MonthlyReportResponseDto> GetMonthlyReportAsync(int year, int month)
        {
            var dailySales = await _context.DailySales
                .Where(ds => ds.SaleDate.Year == year && ds.SaleDate.Month == month)
                .GroupBy(ds => ds.SaleDate)
                .Select(g => new DailySaleSummaryDto
                {
                    Date = g.Key,
                    Day = g.Key.Day,
                    CupsSold = g.Sum(x => x.Quantity),
                    Amount = g.Sum(x => (decimal)(x.Quantity * x.UnitPrice))
                })
                .OrderBy(x => x.Day)
                .ToListAsync();

            var dailyExpenses = await _context.Expenses
                .Where(e => e.Date.Year == year && e.Date.Month == month)
                .GroupBy(e => e.Date)
                .Select(g => new DailyExpenseSummaryDto
                {
                    Date = g.Key,
                    Day = g.Key.Day,
                    Amount = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Day)
                .ToListAsync();

            var totalCupsSold = dailySales.Sum(s => s.CupsSold);
            var totalIncome = dailySales.Sum(s => s.Amount);
            var totalExpense = dailyExpenses.Sum(e => e.Amount);

            return new MonthlyReportResponseDto
            {
                Year = year,
                Month = month,
                TotalCupsSold = totalCupsSold,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                DailySales = dailySales,
                DailyExpenses = dailyExpenses
            };
        }
    }
}
