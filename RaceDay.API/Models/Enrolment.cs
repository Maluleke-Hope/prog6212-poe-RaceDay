namespace RaceDay.API.Models;

public class Enrolment
{
    public int EnrolmentId { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public int CategoryId { get; set; }
    public string Status { get; set; } = "Pending";
    

    public User? User { get; set; }
    public Event? Event { get; set; }
    public Category? Category { get; set; }
    public Result? Result { get; set; } // THIS WAS MISSING
}