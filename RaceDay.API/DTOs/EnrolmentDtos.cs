namespace RaceDay.API.DTOs
{
    public class CreateEnrolmentDto
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }

    public class EnrolmentReadDto
    {
        public int EnrolmentId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; } = "Enrolled";
    }
}