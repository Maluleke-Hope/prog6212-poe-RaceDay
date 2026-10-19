using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 1000.0)]
        public double Distance { get; set; }

        [Required]
        public string EventType { get; set; } = string.Empty; // Run, Walk, Cycle

        [Required]
        public int OrganiserId { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
