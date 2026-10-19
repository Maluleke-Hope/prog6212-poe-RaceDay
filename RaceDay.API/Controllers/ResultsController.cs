using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

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

        [HttpPost]
        public async Task<ActionResult<ResultReadDto>> CreateResult(CreateResultDto dto)
        {
            var result = new Result
            {
                EnrolmentId = dto.EnrolmentId,
                FinishTime = dto.FinishTime,
                Position = dto.Position
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

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