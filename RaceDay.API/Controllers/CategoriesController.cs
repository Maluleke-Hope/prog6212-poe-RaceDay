using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    // Controller responsible for managing event category routes
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        // Inject database context
        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/Categories
        // Retrieves all race categories registered in the database
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

        // POST: api/v1/Categories
        // Creates a new category for a specific event
        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> CreateCategory(CreateCategoryDto dto)
        {
            // Map incoming request DTO to database domain model
            var category = new Category
            {
                Name = dto.Name,
                EventId = dto.EventId
            };

            // Save new category entity to the database
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Map saved entity back to read DTO
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