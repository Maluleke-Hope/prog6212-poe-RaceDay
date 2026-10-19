using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryReadDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    EventId = c.EventId
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> CreateCategory(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                EventId = dto.EventId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var readDto = new CategoryReadDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                EventId = category.EventId
            };

            return CreatedAtAction(nameof(GetCategories), new { id = category.CategoryId }, readDto);
        }
    }
}