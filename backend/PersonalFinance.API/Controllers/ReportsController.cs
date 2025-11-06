using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.DTOs;
using PersonalFinance.Core.Entities;
using PersonalFinance.Core.Interfaces;
using System.Globalization;
using System.Security.Claims;

namespace PersonalFinance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsRepository _reportsRepository;

        public ReportsController(IReportsRepository reportsRepository)
        {
            _reportsRepository = reportsRepository;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        [HttpGet("monthly-summary")]
        public async Task<ActionResult<MonthlySummaryDto>> GetMonthlySummary(
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var userId = GetUserId();

            // Default to current month/year
            var now = DateTime.Now;
            month ??= now.Month;
            year ??= now.Year;

            // Validate
            if (month < 1 || month > 12)
                return BadRequest("Month must be between 1 and 12");

            if (year < 2000 || year > 2100)
                return BadRequest("Invalid year");

            var (totalIncome, totalExpenses, balance, transactionCount) =
                await _reportsRepository.GetMonthlySummaryAsync(userId, month.Value, year.Value);

            var savingsRate = totalIncome > 0
                ? Math.Round((balance / totalIncome) * 100, 2)
                : 0;

            var dto = new MonthlySummaryDto
            {
                Month = month.Value,
                Year = year.Value,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                Balance = balance,
                TransactionCount = transactionCount,
                SavingsRate = savingsRate
            };

            return Ok(dto);
        }

        [HttpGet("spending-by-category")]
        public async Task<ActionResult<SpendingByCategoryReportDto>> GetSpendingByCategory(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = GetUserId();

            // Default to current month if not provided
            if (!startDate.HasValue || !endDate.HasValue)
            {
                var now = DateTime.Now;
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
            }

            if (startDate > endDate)
                return BadRequest("Start date must be before end date");

            var spending = await _reportsRepository.GetSpendingByCategoryAsync(
                userId, startDate.Value, endDate.Value);

            var categories = spending.Select(s => new SpendingByCategoryDto
            {
                CategoryName = s.CategoryName,
                CategoryIcon = s.CategoryIcon,
                Amount = s.Amount,
                Percentage = s.Percentage
            });

            var totalSpending = categories.Sum(c => c.Amount);

            var dto = new SpendingByCategoryReportDto
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalSpending = totalSpending,
                Categories = categories
            };

            return Ok(dto);
        }

        [HttpGet("income-vs-expense-trends")]
        public async Task<ActionResult<IncomeVsExpenseTrendsDto>> GetIncomeVsExpenseTrends(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = GetUserId();

            // Default to last 6 months if not provided
            if (!startDate.HasValue || !endDate.HasValue)
            {
                endDate = DateTime.Now;
                startDate = endDate.Value.AddMonths(-5);
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, 1);
            }

            if (startDate > endDate)
                return BadRequest("Start date must be before end date");

            var trends = await _reportsRepository.GetIncomeVsExpenseTrendsAsync(
                userId, startDate.Value, endDate.Value);

            var trendDtos = trends.Select(t => new MonthlyTrendDto
            {
                Month = t.Month,
                Year = t.Year,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(t.Month),
                Income = t.Income,
                Expenses = t.Expenses,
                NetSavings = t.Income - t.Expenses
            });

            var dto = new IncomeVsExpenseTrendsDto
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Trends = trendDtos
            };

            return Ok(dto);
        }
    }
}
