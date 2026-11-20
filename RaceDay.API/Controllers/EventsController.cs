using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Authorization;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _db;
    private readonly IBlobService _blob;

    public EventsController(RaceDayDbContext db, IBlobService blob)
    {
        _db = db;
        _blob = blob;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Events.Include(e => e.Categories).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ev = await _db.Events.Include(e => e.Categories)
                                 .FirstOrDefaultAsync(e => e.EventId == id);
        return ev is null ? NotFound() : Ok(ev);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Organiser)]
    public async Task<IActionResult> Create(EventCreateDto dto)
    {
        if (dto.Date < DateTime.UtcNow.Date)
            return BadRequest("Event date cannot be in the past.");

        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Date = dto.Date,
            Location = dto.Location,
            Distance = dto.Distance,
            EventType = dto.EventType,
            OrganiserId = CurrentUserId
        };
        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = ev.EventId }, ev);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = AppRoles.Organiser)]
    public async Task<IActionResult> Update(int id, EventCreateDto dto)
    {
        if (dto.Date < DateTime.UtcNow.Date)
            return BadRequest("Event date cannot be in the past.");

        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();

        ev.Title = dto.Title;
        ev.Description = dto.Description;
        ev.Date = dto.Date;
        ev.Location = dto.Location;
        ev.Distance = dto.Distance;
        ev.EventType = dto.EventType;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Organiser)]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();
        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/image")]
    [Authorize(Roles = AppRoles.Organiser)]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != CurrentUserId) return Forbid();
        if (file is null || file.Length == 0) return BadRequest("No file supplied.");

        ev.ImageUrl = await _blob.UploadAsync(file);
        await _db.SaveChangesAsync();
        return Ok(new { ev.ImageUrl });
    }
}