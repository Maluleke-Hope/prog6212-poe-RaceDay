using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
using System.Text.Json;

namespace RaceDay.Web.Controllers;

public class EventsController : Controller
{
    private readonly ApiClient _apiClient;

    public EventsController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserId") == null)
            return RedirectToAction("Login", "Auth");

        var response = await _apiClient.GetAsync("api/events");
        if (!response.IsSuccessStatusCode)
            return Content("Error fetching events.");

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var events = JsonSerializer.Deserialize<List<EventViewModel>>(json, options) ?? new List<EventViewModel>();

        if (HttpContext.Session.GetString("UserRole") == "Participant")
        {
            var enrolResponse = await _apiClient.GetAsync("api/enrolments/mine");
            if (enrolResponse.IsSuccessStatusCode)
            {
                var enrolJson = await enrolResponse.Content.ReadAsStringAsync();
                var myEnrolments = JsonDocument.Parse(enrolJson).RootElement;

                if (myEnrolments.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in myEnrolments.EnumerateArray())
                    {
                        int evId = e.GetProperty("eventId").GetInt32();
                        int enrId = e.GetProperty("enrolmentId").GetInt32();

                        var match = events.FirstOrDefault(ev => ev.EventId == evId);
                        if (match != null)
                        {
                            match.IsEnrolled = true;
                            match.EnrolmentId = enrId;
                        }
                    }
                }
            }
        }

        ViewBag.Role = HttpContext.Session.GetString("UserRole");
        ViewBag.UserName = HttpContext.Session.GetString("UserName");
        ViewBag.CurrentUserId = int.Parse(HttpContext.Session.GetString("UserId")!);

        return View(events);
    }

    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId)
    {
        var response = await _apiClient.PostAsync("api/enrolments", new { eventId, categoryId = 1 });
        if (response.IsSuccessStatusCode) TempData["Success"] = "Enrolled successfully!";
        else TempData["Error"] = "You are already enrolled.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int enrolmentId)
    {
        await _apiClient.DeleteAsync($"api/enrolments/{enrolmentId}");
        TempData["Success"] = "Enrolment cancelled.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Attendees(int id)
    {
        var response = await _apiClient.GetAsync($"api/enrolments/event/{id}");
        if (!response.IsSuccessStatusCode) return Content("Could not load attendees.");

        var json = await response.Content.ReadAsStringAsync();
        var attendees = JsonDocument.Parse(json).RootElement;

        return View(attendees);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(string title, string description, DateTime date, string location, double distance, string eventType)
    {
        var response = await _apiClient.PostAsync("api/events", new { title, description, date, location, distance, eventType });
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ViewBag.Error = await response.Content.ReadAsStringAsync();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var response = await _apiClient.GetAsync($"api/events/{id}");
        if (!response.IsSuccessStatusCode) return NotFound();
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var ev = JsonSerializer.Deserialize<EventViewModel>(json, options);
        return View(ev);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string title, string description, DateTime date, string location, double distance, string eventType)
    {
        var response = await _apiClient.PutAsync($"api/events/{id}", new { title, description, date, location, distance, eventType });
        if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
        ViewBag.Error = await response.Content.ReadAsStringAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _apiClient.DeleteAsync($"api/events/{id}");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            await _apiClient.PostFileAsync($"api/events/{id}/image", file);
        }
        return RedirectToAction(nameof(Index));
    }
}