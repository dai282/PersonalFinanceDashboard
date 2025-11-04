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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var userId = GetUserId();
            var categories = await _categoryRepository.GetAllAsync(userId);

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                Icon = c.Icon,
                IsCustom = c.UserId != null
            });

            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            var dto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                Icon = category.Icon,
                IsCustom = category.UserId != null
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
        {
            var userId = GetUserId();

            if (dto.Type != "Income" && dto.Type != "Expense")
                return BadRequest("Type must be 'Income' or 'Expense'");

            var category = new Category
            {
                Name = dto.Name,
                Type = dto.Type,
                Icon = dto.Icon,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _categoryRepository.CreateAsync(category);

            var responseDto = new CategoryDto
            {
                Id = created.Id,
                Name = created.Name,
                Type = created.Type,
                Icon = created.Icon,
                IsCustom = true
            };

            return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, responseDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, CreateCategoryDto dto)
        {
            var userId = GetUserId();
            var existing = await _categoryRepository.GetByIdAsync(id);

            if (existing == null)
                return NotFound();

            if (existing.UserId != userId)
                return Forbid(); // Can't edit other user's categories or default categories

            existing.Name = dto.Name;
            existing.Icon = dto.Icon;

            var updated = await _categoryRepository.UpdateAsync(existing);

            var responseDto = new CategoryDto
            {
                Id = updated!.Id,
                Name = updated.Name,
                Type = updated.Type,
                Icon = updated.Icon,
                IsCustom = true
            };

            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var userId = GetUserId();
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            if (category.UserId != userId)
                return Forbid();

            var deleted = await _categoryRepository.DeleteAsync(id);
            if (!deleted)
                return BadRequest("Cannot delete default categories");

            return NoContent();
        }
    }
}