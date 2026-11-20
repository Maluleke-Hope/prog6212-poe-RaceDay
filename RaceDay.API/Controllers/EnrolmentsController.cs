using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Authorization;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/enrolments")]
[Authorize]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    public EnrolmentsController(RaceDayDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize(Roles = AppRoles.Participant)]
    public async Task<IActionResult> Enrol(EnrolmentCreateDto dto)
    {
        var exists = await _db.Enrolments
            .AnyAsync(e => e.UserId == CurrentUserId && e.EventId == dto.EventId);
        if (exists) return Conflict("Already enrolled in this event.");

        var enrolment = new Enrolment
        {
            UserId = CurrentUserId,
            EventId = dto.EventId,
            CategoryId = dto.CategoryId,
            Status = "Confirmed"
        };
        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();
        return Ok(enrolment);
    }

    [HttpGet("mine")]
    [Authorize(Roles = AppRoles.Participant)]
    public async Task<IActionResult> Mine()
    {
        var enrolments = await _db.Enrolments
            .Where(e => e.UserId == CurrentUserId)
            .Select(e => new { e.EnrolmentId, e.EventId, e.UserId, e.Status })
            .ToListAsync();
        return Ok(enrolments);
    }

    [HttpGet("event/{eventId}")]
    [Authorize(Roles = AppRoles.Organiser)]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        var attendees = await _db.Enrolments
            .Include(e => e.User)
            .Include(e => e.Category)
            .Where(e => e.EventId == eventId)
            .Select(e => new
            {
                e.EnrolmentId,
                e.EventId,
                e.UserId,
                e.Status,
                UserFullName = e.User != null ? e.User.FullName : "Unknown",
                CategoryName = e.Category != null ? e.Category.Name : "N/A"
            })
            .ToListAsync();

        return Ok(attendees);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Participant)]
    public async Task<IActionResult> Cancel(int id)
    {
        var enrolment = await _db.Enrolments.FindAsync(id);
        if (enrolment is null) return NotFound();
        if (enrolment.UserId != CurrentUserId) return Forbid();

        _db.Enrolments.Remove(enrolment);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}