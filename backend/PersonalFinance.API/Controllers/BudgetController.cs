using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.DTOs;
using PersonalFinance.Core.Entities;
using PersonalFinance.Core.Interfaces;
using System.Security.Claims;

namespace PersonalFinance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;

        public BudgetsController(
            IBudgetRepository budgetRepository,
            ICategoryRepository categoryRepository)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        private async Task<BudgetDto> MapToBudgetDto(Budget budget)
        {
            var spent = await _budgetRepository.GetActualSpendingAsync(
                budget.UserId, budget.CategoryId, budget.Month, budget.Year);

            var remaining = budget.Amount - spent;
            var percentageUsed = budget.Amount > 0 ? (spent / budget.Amount) * 100 : 0;

            string status;
            if (percentageUsed >= 100)
                status = "Over";
            else if (percentageUsed >= 80)
                status = "Near";
            else
                status = "Under";

            return new BudgetDto
            {
                Id = budget.Id,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category.Name,
                CategoryIcon = budget.Category.Icon ?? "",
                Amount = budget.Amount,
                Month = budget.Month,
                Year = budget.Year,
                Spent = spent,
                Remaining = remaining,
                PercentageUsed = Math.Round(percentageUsed, 2),
                Status = status
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetDto>>> GetBudgets(
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var userId = GetUserId();

            // Default to current month/year if not provided
            if (!month.HasValue || !year.HasValue)
            {
                var now = DateTime.Now;
                month ??= now.Month;
                year ??= now.Year;
            }

            var budgets = await _budgetRepository.GetAllAsync(userId, month, year);

            var budgetDtos = new List<BudgetDto>();
            foreach (var budget in budgets)
            {
                var dto = await MapToBudgetDto(budget);
                budgetDtos.Add(dto);
            }

            return Ok(budgetDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BudgetDto>> GetBudget(int id)
        {
            var userId = GetUserId();
            var budget = await _budgetRepository.GetByIdAsync(id, userId);

            if (budget == null)
                return NotFound();

            var dto = await MapToBudgetDto(budget);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<BudgetDto>> CreateBudget(CreateBudgetDto dto)
        {
            var userId = GetUserId();

            // Validate category
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Invalid category");

            if (category.Type != "Expense")
                return BadRequest("Budgets can only be created for expense categories");

            // Validate month and year
            if (dto.Month < 1 || dto.Month > 12)
                return BadRequest("Month must be between 1 and 12");

            if (dto.Year < 2000 || dto.Year > 2100)
                return BadRequest("Invalid year");

            // Validate amount
            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than 0");

            // Check for duplicate
            var existing = await _budgetRepository.GetAllAsync(userId, dto.Month, dto.Year);
            if (existing.Any(b => b.CategoryId == dto.CategoryId))
                return BadRequest("Budget already exists for this category and month");

            var budget = new Budget
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                Month = dto.Month,
                Year = dto.Year,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _budgetRepository.CreateAsync(budget);
            var responseDto = await MapToBudgetDto(created);

            return CreatedAtAction(nameof(GetBudget), new { id = created.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BudgetDto>> UpdateBudget(int id, UpdateBudgetDto dto)
        {
            var userId = GetUserId();
            var existing = await _budgetRepository.GetByIdAsync(id, userId);

            if (existing == null)
                return NotFound();

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than 0");

            existing.Amount = dto.Amount;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _budgetRepository.UpdateAsync(existing);
            var responseDto = await MapToBudgetDto(updated!);

            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBudget(int id)
        {
            var userId = GetUserId();
            var deleted = await _budgetRepository.DeleteAsync(id, userId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("status")]
        public async Task<ActionResult<BudgetStatusDto>> GetBudgetStatus(
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var userId = GetUserId();

            // Default to current month/year
            var now = DateTime.Now;
            month ??= now.Month;
            year ??= now.Year;

            var budgets = await _budgetRepository.GetAllAsync(userId, month, year);

            var budgetDtos = new List<BudgetDto>();
            decimal totalBudgeted = 0;
            decimal totalSpent = 0;

            foreach (var budget in budgets)
            {
                var dto = await MapToBudgetDto(budget);
                budgetDtos.Add(dto);
                totalBudgeted += dto.Amount;
                totalSpent += dto.Spent;
            }

            var statusDto = new BudgetStatusDto
            {
                Month = month.Value,
                Year = year.Value,
                TotalBudgeted = totalBudgeted,
                TotalSpent = totalSpent,
                TotalRemaining = totalBudgeted - totalSpent,
                Budgets = budgetDtos
            };

            return Ok(statusDto);
        }
    }
}