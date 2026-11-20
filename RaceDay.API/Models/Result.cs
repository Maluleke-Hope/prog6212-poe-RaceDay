using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    // Domain model representing the race finish result and ranking for an enrolled participant
    public class Result
    {
        // Primary key for the race result record
        [Key]
        public int ResultId { get; set; }

        // Foreign key referencing the associated participant enrolment
        [Required]
        public int EnrolmentId { get; set; }

        // Navigation property referencing the enrolment entity
        public Enrolment? Enrolment { get; set; }

        // Recorded finish time (e.g., "00:45:30")
        [Required]
        public string FinishTime { get; set; } = string.Empty;

        // Final overall finish position/ranking in the race
        [Required]
        public int Position { get; set; }
    }
}