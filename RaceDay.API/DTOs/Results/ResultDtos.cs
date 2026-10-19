namespace RaceDay.API.DTOs
{
    public class CreateResultDto
    {
        public int EnrolmentId { get; set; }
        public string FinishTime { get; set; } = string.Empty;
        public int Position { get; set; }
    }

    public class ResultReadDto
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string FinishTime { get; set; } = string.Empty;
        public int Position { get; set; }
    }
}