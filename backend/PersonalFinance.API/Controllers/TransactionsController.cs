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
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionsController(
            ITransactionRepository transactionRepository,
            ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepository;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedTransactionsDto>> GetTransactions(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            // Validate category exists
            if (categoryId != null)
            {
                var category = await _categoryRepository.GetByIdAsync((int)categoryId);
                if (category == null)
                    return BadRequest("Invalid category");
            }

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var userId = GetUserId();
            var (transactions, totalCount) = await _transactionRepository.GetAllAsync(
                userId, pageNumber, pageSize, categoryId, startDate, endDate);

            var transactionDtos = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                CategoryIcon = t.Category.Icon ?? "",
                Amount = t.Amount,
                Description = t.Description,
                TransactionDate = t.TransactionDate,
                Type = t.Type,
                CreatedAt = t.CreatedAt
            });

            var response = new PaginatedTransactionsDto
            {
                Transactions = transactionDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(CreateTransactionDto dto)
        {
            var userId = GetUserId();

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Invalid category");

            // Validate amount
            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than 0");

            var transaction = new Transaction
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                Amount = dto.Amount,
                Description = dto.Description,
                TransactionDate = dto.TransactionDate,
                Type = category.Type,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _transactionRepository.CreateAsync(transaction);

            var responseDto = new TransactionDto
            {
                Id = created.Id,
                CategoryId = created.CategoryId,
                CategoryName = created.Category.Name,
                CategoryIcon = created.Category.Icon ?? "",
                Amount = created.Amount,
                Description = created.Description,
                TransactionDate = created.TransactionDate,
                Type = created.Type,
                CreatedAt = created.CreatedAt
            };

            return CreatedAtAction(nameof(GetTransaction), new { id = created.Id }, responseDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            var userId = GetUserId();
            var transaction = await _transactionRepository.GetByIdAsync(id, userId);

            if (transaction == null)
                return NotFound();

            var dto = new TransactionDto
            {
                Id = transaction.Id,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category.Name,
                CategoryIcon = transaction.Category.Icon ?? "",
                Amount = transaction.Amount,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                Type = transaction.Type,
                CreatedAt = transaction.CreatedAt
            };

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = GetUserId();
            var success = await _transactionRepository.DeleteAsync(id, userId);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDto>> UpdateTransaction(int id, UpdateTransactionDto dto)
        {
            var userId = GetUserId();

            var existing = await _transactionRepository.GetByIdAsync(id, userId);
            if (existing == null)
                return NotFound();

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Invalid category");

            // Validate amount
            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than 0");

            existing.CategoryId = dto.CategoryId;
            existing.Amount = dto.Amount;
            existing.Description = dto.Description;
            existing.TransactionDate = dto.TransactionDate;
            existing.Type = category.Type;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _transactionRepository.UpdateAsync(existing);

            var responseDto = new TransactionDto
            {
                Id = updated.Id,
                CategoryId = updated.CategoryId,
                CategoryName = updated.Category.Name,
                CategoryIcon = updated.Category.Icon ?? "",
                Amount = updated.Amount,
                Description = updated.Description,
                TransactionDate = updated.TransactionDate,
                Type = updated.Type,
                CreatedAt = updated.CreatedAt
            };

            return Ok(responseDto);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<TransactionStatisticsDto>> GetStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = GetUserId();
            var (totalIncome, totalExpenses, balance) = await _transactionRepository.GetStatisticsAsync(
                userId, startDate, endDate);

            var dto = new TransactionStatisticsDto
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                Balance = balance,
                StartDate = startDate,
                EndDate = endDate
            };

            return Ok(dto);
        }
    }
}
