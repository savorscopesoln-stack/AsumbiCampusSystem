using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsumbiCampusSystem.Models
{
    public class MealRecord
    {
        [Key]
        public int Id { get; set; }

        // Link to Student
        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public virtual Student? Student { get; set; }

        [Required]
        public string StudentName { get; set; } = "";

        [Required]
        public string AdmissionNumber { get; set; } = "";

        [Required]
        public string MealType { get; set; } = ""; // Breakfast, Lunch, Supper

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "Served"; // Served, Denied

        // True if attempted without valid meal card
        public bool AttemptedWithoutCard { get; set; } = false;

        // Staff who scanned
        [Required]
        [ForeignKey("Staff")]
        public int StaffId { get; set; }
        public virtual User? Staff { get; set; }

        // Optional remarks
        public string Remarks { get; set; } = "";
    }
}