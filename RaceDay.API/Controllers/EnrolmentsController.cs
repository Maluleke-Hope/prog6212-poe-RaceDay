using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrolmentReadDto>>> GetEnrolments()
        {
            var enrolments = await _context.Enrolments
                .Select(e => new EnrolmentReadDto
                {
                    EnrolmentId = e.EnrolmentId,
                    UserId = e.UserId,
                    EventId = e.EventId,
                    CategoryId = e.CategoryId,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        [HttpPost]
        public async Task<ActionResult<EnrolmentReadDto>> CreateEnrolment(CreateEnrolmentDto dto)
        {
            var enrolment = new Enrolment
            {
                UserId = dto.UserId,
                EventId = dto.EventId,
                CategoryId = dto.CategoryId,
                Status = "Enrolled"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            var readDto = new EnrolmentReadDto
            {
                EnrolmentId = enrolment.EnrolmentId,
                UserId = enrolment.UserId,
                EventId = enrolment.EventId,
                CategoryId = enrolment.CategoryId,
                Status = enrolment.Status
            };

            return CreatedAtAction(nameof(GetEnrolments), new { id = enrolment.EnrolmentId }, readDto);
        }
    }
}