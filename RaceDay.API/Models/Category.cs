using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    // Represents a distance or division category for a race event
    public class Category
    {
        // Primary key for the category record
        [Key]
        public int CategoryId { get; set; }

        // Category display name (e.g., Open Male, 10km Elite)
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Foreign key linking the category to a specific race event
        [Required]
        public int EventId { get; set; }

        // Navigation property referencing the parent Event entity
        [ForeignKey("EventId")]
        public Event Event { get; set; }
    }
}