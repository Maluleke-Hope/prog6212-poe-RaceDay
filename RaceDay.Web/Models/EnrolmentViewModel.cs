namespace RaceDay.Web.Models;

public class EnrolmentViewModel
{
    public int EnrolmentId { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public UserViewModel? User { get; set; }
    public CategoryViewModel? Category { get; set; }
}

public class UserViewModel { public string FullName { get; set; } = string.Empty; }
public class CategoryViewModel { public string Name { get; set; } = string.Empty; }