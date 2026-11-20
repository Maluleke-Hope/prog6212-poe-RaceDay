namespace RaceDay.Web.Models;

public class EventViewModel
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Location { get; set; } = string.Empty;
    public double Distance { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int OrganiserId { get; set; }

    public bool IsEnrolled { get; set; }
    public int EnrolmentId { get; set; }
}