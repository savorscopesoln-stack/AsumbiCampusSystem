using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsumbiCampusSystem.Models
{
    public class MealCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public string IssuedBy { get; set; } = "";

        public string Remarks { get; set; } = "";

        // Navigation property to Student
        public virtual Student? Student { get; set; }
    }
}