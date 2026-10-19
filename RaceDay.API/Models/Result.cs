using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }
        public Enrolment? Enrolment { get; set; }

        [Required]
        public string FinishTime { get; set; } = string.Empty;

        [Required]
        public int Position { get; set; }
    }
}