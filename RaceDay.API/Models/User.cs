using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    // Domain model representing system users (Organisers and Participants)
    public class User
    {
        // Primary key for the user account
        [Key]
        public int UserId { get; set; }

        // User's full name
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // User's email address used for identification
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Securely hashed password credential
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // System authorization role (defaults to "Participant", can be "Organiser")
        [Required]
        public string Role { get; set; } = "Participant";

        // Navigation collection linking the user to their event enrolments
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}