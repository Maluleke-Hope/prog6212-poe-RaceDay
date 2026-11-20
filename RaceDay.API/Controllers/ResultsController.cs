using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    // Controller responsible for logging and retrieving race finish times and rankings
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        // Inject database context
        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/Results
        // Retrieves recorded race results for participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultReadDto>>> GetResults()
        {
            var results = await _context.Results
                .Select(r => new ResultReadDto
                {
                    ResultId = r.ResultId,
                    EnrolmentId = r.EnrolmentId,
                    FinishTime = r.FinishTime,
                    Position = r.Position
                })
                .ToListAsync();

            return Ok(results);
        }

        // POST: api/v1/Results
        // Logs a new finish time and position for an enrolled participant
        [HttpPost]
        public async Task<ActionResult<ResultReadDto>> CreateResult(CreateResultDto dto)
        {
            // Map incoming request data to database domain model
            var result = new Result
            {
                EnrolmentId = dto.EnrolmentId,
                FinishTime = dto.FinishTime,
                Position = dto.Position
            };

            // Save new race result entry to the database
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            // Map saved entity back to read DTO for response output
            var readDto = new ResultReadDto
            {
                ResultId = result.ResultId,
                EnrolmentId = result.EnrolmentId,
                FinishTime = result.FinishTime,
                Position = result.Position
            };

            return CreatedAtAction(nameof(GetResults), new { id = result.ResultId }, readDto);
        }
    }
}